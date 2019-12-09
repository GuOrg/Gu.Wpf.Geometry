namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// A decorator that adds zoom and pan.
    /// </summary>
    public class Zoombox : Decorator
    {
        /// <summary>Identifies the <see cref="WheelZoomFactor"/> dependency property.</summary>
        public static readonly DependencyProperty WheelZoomFactorProperty = DependencyProperty.Register(
            nameof(WheelZoomFactor),
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(1.05));

        /// <summary>Identifies the <see cref="MinZoom"/> dependency property.</summary>
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(
            nameof(MinZoom),
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(double.NegativeInfinity));

        /// <summary>Identifies the <see cref="MaxZoom"/> dependency property.</summary>
        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(
            nameof(MaxZoom),
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(double.PositiveInfinity));

        /// <summary>Identifies the <see cref="ContentMatrix"/> dependency property.</summary>
        public static readonly DependencyProperty ContentMatrixProperty = DependencyProperty.Register(
            nameof(ContentMatrix),
            typeof(Matrix),
            typeof(Zoombox),
            new PropertyMetadata(
                default(Matrix),
                (d, e) => ((MatrixTransform)((Zoombox)d).InternalVisual.Transform).SetCurrentValue(MatrixTransform.MatrixProperty, (Matrix)e.NewValue)));

        private static readonly ScaleTransform ScaleTransform = new ScaleTransform();
        private static readonly TranslateTransform TranslateTransform = new TranslateTransform();
        private static readonly double DefaultScaleIncrement = 2.0;
        private static readonly double MinScaleDelta = 1E-6;

        private ContainerVisual internalVisual;
        private Point position;

        static Zoombox()
        {
            ClipToBoundsProperty.OverrideMetadata(
                typeof(Zoombox),
                new PropertyMetadata(
                    true,
                    ClipToBoundsProperty.GetMetadata(typeof(Decorator)).PropertyChangedCallback));
            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    NavigationCommands.IncreaseZoom,
                    OnIncreaseZoom,
                    OnCanIncreaseZoom));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    ZoomCommands.Increase,
                    OnIncreaseZoom,
                    OnCanIncreaseZoom));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    NavigationCommands.DecreaseZoom,
                    OnDecreaseZoom,
                    OnCanDecreaseZoom));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    ZoomCommands.Decrease,
                    OnDecreaseZoom,
                    OnCanDecreaseZoom));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    ZoomCommands.None,
                    OnZoomNone,
                    OnCanZoomNone));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    ZoomCommands.Uniform,
                    OnZoomUniform,
                    OnCanZoomUniform));

            CommandManager.RegisterClassCommandBinding(
                typeof(Zoombox),
                new CommandBinding(
                    ZoomCommands.UniformToFill,
                    OnZoomUniformToFill,
                    OnCanZoomUniformToFill));
        }

        /// <summary>
        /// The increment zoom is changed on each mouse wheel.
        /// </summary>
        public double WheelZoomFactor
        {
            get => (double)this.GetValue(WheelZoomFactorProperty);
            set => this.SetValue(WheelZoomFactorProperty, value);
        }

        /// <summary>
        /// The minimum zoom allowed.
        /// </summary>
        public double MinZoom
        {
            get => (double)this.GetValue(MinZoomProperty);
            set => this.SetValue(MinZoomProperty, value);
        }

        /// <summary>
        /// The maximum zoom allowed.
        /// </summary>
        public double MaxZoom
        {
            get => (double)this.GetValue(MaxZoomProperty);
            set => this.SetValue(MaxZoomProperty, value);
        }

        /// <summary>
        /// The transform applied to the contents.
        /// </summary>
        public Matrix ContentMatrix
        {
            get => (Matrix)this.GetValue(ContentMatrixProperty);
            set => this.SetValue(ContentMatrixProperty, value);
        }

        /// <inheritdoc />
        public override UIElement Child
        {
            // everything is the same as on Decorator, the only difference is to insert intermediate Visual to
            // specify scaling transform
            get => this.InternalChild;

            set
            {
                var old = this.InternalChild;

                if (!ReferenceEquals(old, value))
                {
                    // need to remove old element from logical tree
                    this.RemoveLogicalChild(old);

                    if (value != null)
                    {
                        this.AddLogicalChild(value);
                    }

                    this.InternalChild = value;

                    this.InvalidateMeasure();
                }
            }
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => 1;

        /// <summary>
        /// Returns enumerator to logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.InternalChild == null)
                {
                    return EmptyEnumerator.Instance;
                }

                return new SingleChildEnumerator(this.InternalChild);
            }
        }

        private ContainerVisual InternalVisual
        {
            get
            {
                if (this.internalVisual == null)
                {
                    this.internalVisual = new ContainerVisual
                    {
                        Transform = new MatrixTransform(Matrix.Identity),
                    };
                    this.AddVisualChild(this.internalVisual);
                }

                return this.internalVisual;
            }
        }

        private Vector CurrentZoom => new Vector(this.ContentMatrix.M11, this.ContentMatrix.M22);

        private UIElement InternalChild
        {
            get
            {
                var vc = this.InternalVisual.Children;
                if (vc.Count != 0)
                {
                    return vc[0] as UIElement;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                var vc = this.InternalVisual.Children;
                if (vc.Count != 0)
                {
                    vc.Clear();
                }

                vc.Add(value);
            }
        }

        /// <summary>
        /// Zoom around a the center of the currently visible part.
        /// </summary>
        /// <param name="scale">The amount to resize as a multiplier.</param>
        public void Zoom(double scale)
        {
            this.Zoom(new Vector(scale, scale));
        }

        /// <summary>
        /// Zoom around a the center of the currently visible part.
        /// </summary>
        /// <param name="scale">The amount to resize as a multipliers.</param>
        public void Zoom(Vector scale)
        {
            var point = LayoutInformation.GetLayoutClip(this)?.Bounds.CenterPoint();
            this.Zoom(point ?? new Point(0, 0), scale);
        }

        /// <summary>
        /// Zoom around a point.
        /// </summary>
        /// <param name="center">The point to zoom about.</param>
        /// <param name="scale">The amount to resize as a multipliers.</param>
        public void Zoom(Point center, Vector scale)
        {
            scale = this.CoerceScale(scale);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, center.X);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, center.Y);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, scale.X);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, scale.Y);
            this.SetCurrentValue(ContentMatrixProperty, Matrix.Multiply(this.ContentMatrix, ScaleTransform.Value));
        }

        /// <summary>
        /// The content is re-sized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        public void ZoomUniform()
        {
            if (this.InternalChild == null)
            {
                return;
            }

            if (!this.InternalChild.IsArrangeValid)
            {
                _ = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.ZoomUniform()));
                return;
            }

            var size = this.InternalChild.DesiredSize;
            if (Math.Abs(size.Width) < MinScaleDelta ||
                Math.Abs(size.Height) < MinScaleDelta)
            {
                return;
            }

            var scaleX = this.ActualWidth / size.Width;
            var scaleY = this.ActualHeight / size.Height;
            var scale = Math.Min(scaleX, scaleY);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, 0.0);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, 0.0);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, scale);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, scale);
            TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, (this.ActualWidth - (scale * size.Width)) / 2);
            TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, (this.ActualHeight - (scale * size.Height)) / 2);
            this.SetCurrentValue(ContentMatrixProperty, Matrix.Multiply(ScaleTransform.Value, TranslateTransform.Value));
        }

        /// <summary>
        /// The content is re-sized to fill the destination dimensions while it preserves its native aspect ratio.
        /// If the aspect ratio of the destination rectangle differs from the source, the source content is clipped to fit in the destination dimensions.
        /// </summary>
        public void ZoomUniformToFill()
        {
            if (this.InternalChild == null)
            {
                return;
            }

            if (!this.InternalChild.IsArrangeValid)
            {
                _ = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.ZoomUniformToFill()));
                return;
            }

            var size = this.InternalChild.DesiredSize;
            if (Math.Abs(size.Width) < MinScaleDelta ||
                Math.Abs(size.Height) < MinScaleDelta)
            {
                return;
            }

            var scaleX = this.ActualWidth / size.Width;
            var scaleY = this.ActualHeight / size.Height;
            var scale = Math.Max(scaleX, scaleY);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, 0.0);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, 0.0);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, scale);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, scale);
            TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, (this.ActualWidth - (scale * size.Width)) / 2);
            TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, (this.ActualHeight - (scale * size.Height)) / 2);
            this.SetCurrentValue(ContentMatrixProperty, Matrix.Multiply(ScaleTransform.Value, TranslateTransform.Value));
        }

        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        public void ZoomNone()
        {
            this.SetCurrentValue(ContentMatrixProperty, Matrix.Identity);
        }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Always exactly one child");
            }

            return this.InternalVisual;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            this.InternalChild?.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height)
                ? this.InternalChild?.DesiredSize ?? default(Size)
                : constraint;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var child = this.InternalChild;
            child?.Arrange(new Rect(child.DesiredSize));
            return arrangeSize;
        }

        /// <inheritdoc />
        protected override void OnRender(DrawingContext dc)
        {
            if (dc is null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
        }

        /// <inheritdoc />
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var delta = e.DeltaManipulation;
            if (Math.Abs(delta.Scale.LengthSquared - 2) > MinScaleDelta)
            {
                var p = ((FrameworkElement)e.ManipulationContainer).TranslatePoint(e.ManipulationOrigin, this);
                this.Zoom(p, delta.Scale);
            }

            if (delta.Translation.LengthSquared > 0)
            {
                TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, delta.Translation.X);
                TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, delta.Translation.Y);
                this.SetCurrentValue(ContentMatrixProperty, Matrix.Multiply(this.ContentMatrix, TranslateTransform.Value));
            }

            // Calling InvalidateRequerySuggested as we are using RoutedCommands
            CommandManager.InvalidateRequerySuggested();
            base.OnManipulationDelta(e);
        }

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (e.Delta == 0 || this.WheelZoomFactor == 1)
            {
                return;
            }

            var scale = e.Delta > 0
                ? this.WheelZoomFactor
                : 1.0 / this.WheelZoomFactor;
            var p = e.GetPosition(this);
            this.Zoom(p, new Vector(scale, scale));

            // Calling InvalidateRequerySuggested as we are using RoutedCommands
            CommandManager.InvalidateRequerySuggested();
            base.OnMouseWheel(e);
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            this.position = e.GetPosition(this);
            _ = this.CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (this.IsMouseCaptured)
            {
                var newPos = e.GetPosition(this);
                var delta = newPos - this.position;
                TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, delta.X);
                TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, delta.Y);
                this.SetCurrentValue(ContentMatrixProperty, Matrix.Multiply(this.ContentMatrix, TranslateTransform.Value));
                this.position = newPos;
            }

            base.OnMouseMove(e);
        }

        private static void OnCanDecreaseZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            var scale = GetScale(e.Parameter);
            scale = scale.LengthSquared > 2
                ? new Vector(1 / scale.X, 1 / scale.Y)
                : scale;
            e.CanExecute = Math.Abs(box.CoerceScale(scale).LengthSquared - 2) > MinScaleDelta;
            e.Handled = true;
        }

        private static void OnDecreaseZoom(object sender, ExecutedRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            var scale = GetScale(e.Parameter);
            scale = scale.LengthSquared > 2
                ? new Vector(1 / scale.X, 1 / scale.Y)
                : scale;
            box.Zoom(scale);
            e.Handled = true;
        }

        private static void OnCanIncreaseZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            var scale = GetScale(e.Parameter);
            e.CanExecute = Math.Abs(box.CoerceScale(scale).LengthSquared - 2) > MinScaleDelta;
            e.Handled = true;
        }

        private static void OnIncreaseZoom(object sender, ExecutedRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            var scale = GetScale(e.Parameter);
            box.Zoom(scale);
            e.Handled = true;
        }

        private static void OnCanZoomNone(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            e.CanExecute = box.InternalChild != null;
            e.Handled = true;
        }

        private static void OnZoomNone(object sender, ExecutedRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            box.ZoomNone();
            e.Handled = true;
        }

        private static void OnCanZoomUniform(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            if (box.InternalChild is UIElement child)
            {
                e.CanExecute = child.DesiredSize.Width > MinScaleDelta &&
                               child.DesiredSize.Height > MinScaleDelta;
            }

            e.Handled = true;
        }

        private static void OnZoomUniform(object sender, ExecutedRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            if (box.InternalChild != null)
            {
                box.ZoomUniform();
                e.Handled = true;
            }
        }

        private static void OnCanZoomUniformToFill(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            if (box.InternalChild is UIElement child)
            {
                e.CanExecute = child.DesiredSize.Width > MinScaleDelta &&
                               child.DesiredSize.Height > MinScaleDelta;
            }

            e.Handled = true;
        }

        private static void OnZoomUniformToFill(object sender, ExecutedRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            if (box.InternalChild != null)
            {
                box.ZoomUniformToFill();
                e.Handled = true;
            }
        }

        private static double Clamp(double min, double value, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        private static Vector GetScale(object parameter)
        {
            return parameter switch
            {
                int i => new Vector(i, i),
                double d => new Vector(d, d),
                Vector v => v,
                _ => new Vector(DefaultScaleIncrement, DefaultScaleIncrement),
            };
        }

        private Vector CoerceScale(Vector scale)
        {
            var zoom = this.CurrentZoom;
            return new Vector(
                Clamp(this.MinZoom / zoom.X, scale.X, this.MaxZoom / zoom.X),
                Clamp(this.MinZoom / zoom.Y, scale.Y, this.MaxZoom / zoom.X));
        }
    }
}
