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
        }

        /// <summary>
        /// The increment zoom is changed on each mouse wheel.
        /// </summary>
        public double ZoomFactor
        {
            get => (double)this.GetValue(ZoomFactorProperty);
            set => this.SetValue(ZoomFactorProperty, value);
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
                ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, p.X);
                ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, p.Y);
                ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, delta.Scale.X);
                ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, delta.Scale.Y);
                this.InternalTransform.SetCurrentValue(MatrixTransform.MatrixProperty, Matrix.Multiply(this.InternalTransform.Value, ScaleTransform.Value));
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
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterXProperty, p.X);
            ScaleTransform.SetCurrentValue(ScaleTransform.CenterYProperty, p.Y);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, scale);
            ScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, scale);
            this.InternalTransform.SetCurrentValue(MatrixTransform.MatrixProperty, Matrix.Multiply(this.InternalTransform.Value, ScaleTransform.Value));
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
    }
}