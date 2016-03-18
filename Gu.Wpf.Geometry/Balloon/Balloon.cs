namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public abstract class Balloon : Shape
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                default(CornerRadius),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnCornerRadiusChanged));

        public static readonly DependencyProperty ConnectorOffsetProperty = DependencyProperty.Register(
            "ConnectorOffset",
            typeof(Vector),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                default(Vector),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        public static readonly DependencyProperty ConnectorAngleProperty = DependencyProperty.Register(
            "ConnectorAngle",
            typeof(double),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                15.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            "PlacementTarget",
            typeof(UIElement),
            typeof(Balloon),
            new PropertyMetadata(default(UIElement), OnPlacementTargetChanged));

        public static readonly DependencyProperty PlacementOptionsProperty = DependencyProperty.Register(
            "PlacementOptions",
            typeof(PlacementOptions),
            typeof(Balloon),
            new PropertyMetadata(default(PlacementOptions), OnPlacementOptionsChanged));

        private readonly PenCache penCache = new PenCache();
        private Geometry balloonGeometry;
        private Geometry boxGeometry;
        private Geometry connectorGeometry;

        static Balloon()
        {
            StretchProperty.OverrideMetadata(typeof(Balloon), new FrameworkPropertyMetadata(Stretch.Fill));
            EventManager.RegisterClassHandler(typeof(Balloon), LoadedEvent, new RoutedEventHandler(OnLoaded));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        public Vector ConnectorOffset
        {
            get { return (Vector)this.GetValue(ConnectorOffsetProperty); }
            set { this.SetValue(ConnectorOffsetProperty, value); }
        }

        public double ConnectorAngle
        {
            get { return (double)this.GetValue(ConnectorAngleProperty); }
            set { this.SetValue(ConnectorAngleProperty, value); }
        }

        public UIElement PlacementTarget
        {
            get { return (UIElement)this.GetValue(PlacementTargetProperty); }
            set { this.SetValue(PlacementTargetProperty, value); }
        }

        public PlacementOptions PlacementOptions
        {
            get { return (PlacementOptions)this.GetValue(PlacementOptionsProperty); }
            set { this.SetValue(PlacementOptionsProperty, value); }
        }

        protected override Geometry DefiningGeometry => this.boxGeometry ?? Geometry.Empty;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.StrokeThickness, this.StrokeThickness);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = this.penCache.GetPen(this.Stroke, this.StrokeThickness);
            drawingContext.DrawGeometry(this.Fill, pen, this.balloonGeometry);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.UpdateCachedGeometries();
            this.InvalidateVisual();
        }

        protected virtual void UpdateCachedGeometries()
        {
            if (this.RenderSize == Size.Empty)
            {
                this.boxGeometry = Geometry.Empty;
                this.connectorGeometry = Geometry.Empty;
                this.balloonGeometry = Geometry.Empty;
                return;
            }

            this.boxGeometry = this.CreateBoxGeometry(this.RenderSize);
            this.connectorGeometry = this.CreateConnectorGeometry(this.RenderSize);
            this.balloonGeometry = this.CreateGeometry(this.boxGeometry, this.connectorGeometry);
        }

        protected abstract Geometry CreateBoxGeometry(Size renderSize);

        protected abstract Geometry CreateConnectorGeometry(Size renderSize);

        protected virtual Geometry CreateGeometry(Geometry boxGeometry, Geometry connectorGeometry)
        {
            var ballonGeometry = new CombinedGeometry(GeometryCombineMode.Union, boxGeometry, connectorGeometry);
            ballonGeometry.Freeze();
            return ballonGeometry;
        }

        protected abstract void UpdateConnectorOffset();

        protected virtual void OnLayoutUpdated(object _, EventArgs __)
        {
            this.UpdateConnectorOffset();
        }

        protected virtual void OnLoaded()
        {
            this.UpdateConnectorOffset();
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private static void OnConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private static void OnPlacementOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            balloon.OnLayoutUpdated(null, null);
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            balloon.UpdateConnectorOffset();
            // unsubscribing and subscribing here to have only one subscription
            balloon.LayoutUpdated -= balloon.OnLayoutUpdated;
            balloon.LayoutUpdated += balloon.OnLayoutUpdated;
            WeakEventManager<UIElement, EventArgs>.RemoveHandler((UIElement)e.OldValue, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
            WeakEventManager<UIElement, EventArgs>.AddHandler((UIElement)e.NewValue, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((Balloon)sender).OnLoaded();
        }

        private class PenCache
        {
            private Brush brush;
            private double strokeThickness;
            private Pen pen;

            public Pen GetPen(Brush brush, double strokeThickness)
            {
                if (this.brush == brush && this.strokeThickness == strokeThickness)
                {
                    return this.pen;
                }

                this.brush = brush;
                this.strokeThickness = strokeThickness;
                this.pen = new Pen(brush, strokeThickness);
                return this.pen;
            }
        }
    }
}