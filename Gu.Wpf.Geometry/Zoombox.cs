namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class Zoombox : Decorator
    {
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            "ZoomFactor",
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(1.05));

        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(
            "MinZoom",
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(double.NegativeInfinity));

        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(
            "MaxZoom",
            typeof(double),
            typeof(Zoombox),
            new PropertyMetadata(double.PositiveInfinity));

        private static readonly ScaleTransform ScaleTransform = new ScaleTransform();
        private static readonly TranslateTransform TranslateTransform = new TranslateTransform();

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
        }

        /// <summary>
        /// The increment zoom is changed on each mouse wheel.
        /// </summary>
        public double ZoomFactor
        {
            get => (double)this.GetValue(ZoomFactorProperty);
            set => this.SetValue(ZoomFactorProperty, value);
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
                        Transform = new MatrixTransform(Matrix.Identity)
                    };
                    this.AddVisualChild(this.internalVisual);
                }

                return this.internalVisual;
            }
        }

        private MatrixTransform InternalTransform => (MatrixTransform)this.internalVisual.Transform;

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
        /// Zoom around a point.
        /// </summary>
        /// <param name="center">The point to zoom about</param>
        /// <param name="scales">The factors to update the zoom with.</param>
        public void Zoom(Point center, Vector scales)
        {
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, center.X);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, center.Y);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, scales.X);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, scales.Y);
            this.InternalTransform.SetCurrentValue(MatrixTransform.MatrixProperty, Matrix.Multiply(this.InternalTransform.Value, ScaleTransform.Value));
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
            return constraint;
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
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            var delta = e.DeltaManipulation;
            if (delta.Scale.LengthSquared > 0)
            {
                var p = ((FrameworkElement)e.ManipulationContainer).TranslatePoint(e.ManipulationOrigin, this);
                this.Zoom(p, delta.Scale);
            }

            if (delta.Translation.LengthSquared > 0)
            {
                TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, delta.Translation.X);
                TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, delta.Translation.Y);
                this.InternalTransform.SetCurrentValue(MatrixTransform.MatrixProperty, Matrix.Multiply(this.InternalTransform.Value, TranslateTransform.Value));
            }

            base.OnManipulationDelta(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (e.Delta == 0 || this.ZoomFactor == 1)
            {
                return;
            }

            var scale = e.Delta > 0
                ? this.ZoomFactor
                : 1.0 / this.ZoomFactor;
            var p = e.GetPosition(this);
            this.Zoom(p, new Vector(scale, scale));
            base.OnMouseWheel(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.position = e.GetPosition(this);
            this.CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                var newPos = e.GetPosition(this);
                var delta = newPos - this.position;
                TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, delta.X);
                TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, delta.Y);
                this.InternalTransform.SetCurrentValue(MatrixTransform.MatrixProperty, Matrix.Multiply(this.InternalTransform.Value, TranslateTransform.Value));
                this.position = newPos;
            }

            base.OnMouseMove(e);
        }

        private static void OnCanDecreaseZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            var box = (Zoombox)e.Source;
            e.CanExecute = box.CanIncreaseZoom();
            e.Handled = true;
        }

        private bool CanIncreaseZoom()
        {
            return this.InternalTransform.Value.M11 < this.MaxZoom &&
                   this.InternalTransform.Value.M22 < this.MaxZoom;
        }

        private static void OnDecreaseZoom(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnCanIncreaseZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnIncreaseZoom(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}