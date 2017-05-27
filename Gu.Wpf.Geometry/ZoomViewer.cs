namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class ZoomViewer : Decorator
    {
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            "ZoomFactor",
            typeof(double),
            typeof(ZoomViewer),
            new PropertyMetadata(1.05));

        public static readonly DependencyProperty ContentScaleTransformProperty = DependencyProperty.Register(
            "ContentScaleTransform",
            typeof(ScaleTransform),
            typeof(ZoomViewer),
            new PropertyMetadata(
                default(ScaleTransform),
                OnContentScaleTransformChanged,
                CoerceContentScaleTransform));

        public static readonly DependencyProperty ContentTranslateTransformProperty = DependencyProperty.Register(
            "ContentTranslateTransform",
            typeof(TranslateTransform),
            typeof(ZoomViewer),
            new PropertyMetadata(
                default(TranslateTransform),
                OnContentTranslateTransformChanged,
                CoerceContentTranslateTransform));

        private readonly TransformGroup transformGroup = new TransformGroup { Children = { Transform.Identity, Transform.Identity } };
        private Point position;
        private bool isUpdating;

        public ZoomViewer()
        {
            this.ContentScaleTransform = new ScaleTransform();
            this.ContentTranslateTransform = new TranslateTransform();
        }

        /// <summary>
        /// The increment zoom is changed on each mouse wheel.
        /// </summary>
        public double ZoomFactor
        {
            get => (double)this.GetValue(ZoomFactorProperty);
            set => this.SetValue(ZoomFactorProperty, value);
        }

        public TranslateTransform ContentTranslateTransform
        {
            get => (TranslateTransform)this.GetValue(ContentTranslateTransformProperty);
            set => this.SetValue(ContentTranslateTransformProperty, value);
        }

        public ScaleTransform ContentScaleTransform
        {
            get => (ScaleTransform)this.GetValue(ContentScaleTransformProperty);
            set => this.SetValue(ContentScaleTransformProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.Child?.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (this.Child != null)
            {
                var rect = new Rect(this.DesiredSize);
                rect.Transform(this.transformGroup.Value);
                this.Child?.Arrange(rect);
            }

            return arrangeSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (e.Delta == 0 || this.ZoomFactor == 1)
            {
                return;
            }

            var newZoom = e.Delta > 0
                ? this.ContentScaleTransform.ScaleX *= this.ZoomFactor
                : this.ContentScaleTransform.ScaleX *= 1.0 / this.ZoomFactor;
            var p = e.GetPosition(this);
            var x = (p.X * this.ContentScaleTransform.ScaleX) + this.ContentTranslateTransform.X;
            var y = (p.Y * this.ContentScaleTransform.ScaleY) + this.ContentTranslateTransform.Y;

            this.isUpdating = true;
            this.ContentScaleTransform.SetCurrentValue(ScaleTransform.ScaleXProperty, newZoom);
            this.ContentScaleTransform.SetCurrentValue(ScaleTransform.ScaleYProperty, newZoom);
            this.ContentTranslateTransform.SetCurrentValue(TranslateTransform.XProperty, x - (p.X * this.ContentScaleTransform.ScaleX));
            this.ContentTranslateTransform.SetCurrentValue(TranslateTransform.YProperty, y - (p.Y * this.ContentScaleTransform.ScaleY));
            this.isUpdating = false;
            this.InvalidateVisual();
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
                this.isUpdating = true;
                this.ContentTranslateTransform.SetCurrentValue(TranslateTransform.XProperty, this.ContentTranslateTransform.X + delta.X);
                this.ContentTranslateTransform.SetCurrentValue(TranslateTransform.YProperty, this.ContentTranslateTransform.Y + delta.Y);
                this.isUpdating = false;
                this.InvalidateVisual();
                this.position = newPos;
            }

            base.OnMouseMove(e);
        }

        private static object CoerceContentTranslateTransform(DependencyObject d, object basevalue)
        {
            return basevalue ?? new TranslateTransform();
        }

        private static void OnContentTranslateTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (ZoomViewer)d;
            if (e.OldValue is TranslateTransform old)
            {
                old.Changed -= viewer.OnTransformChanged;
            }

            var transform = (TranslateTransform)e.NewValue;
            if (transform != null)
            {
                transform.Changed += viewer.OnTransformChanged;
            }

            viewer.transformGroup.Children[1] = transform;
        }

        private static object CoerceContentScaleTransform(DependencyObject d, object basevalue)
        {
            return basevalue ?? new ScaleTransform();
        }

        private static void OnContentScaleTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (ZoomViewer)d;
            if (e.OldValue is ScaleTransform old)
            {
                old.Changed -= viewer.OnTransformChanged;
            }

            var transform = (ScaleTransform)e.NewValue;
            if (transform != null)
            {
                transform.Changed += viewer.OnTransformChanged;
            }

            viewer.transformGroup.Children[0] = transform;
        }

        private void OnTransformChanged(object sender, EventArgs e)
        {
            if (!this.isUpdating)
            {
                this.InvalidateVisual();
            }
        }
    }
}
