namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public abstract class BalloonBase : Shape
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(BalloonBase),
            new FrameworkPropertyMetadata(
                default(CornerRadius),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnCornerRadiusChanged));

        public static readonly DependencyProperty ConnectorOffsetProperty = DependencyProperty.Register(
            "ConnectorOffset",
            typeof(Vector),
            typeof(BalloonBase),
            new FrameworkPropertyMetadata(
                default(Vector),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        public static readonly DependencyProperty ConnectorAngleProperty = DependencyProperty.Register(
            "ConnectorAngle",
            typeof(double),
            typeof(BalloonBase),
            new FrameworkPropertyMetadata(
                15.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            "PlacementTarget",
            typeof(UIElement),
            typeof(BalloonBase),
            new PropertyMetadata(default(UIElement), OnPlacementTargetChanged));

        public static readonly DependencyProperty PlacementOptionsProperty = DependencyProperty.Register(
            "PlacementOptions",
            typeof(PlacementOptions),
            typeof(BalloonBase),
            new PropertyMetadata(Wpf.Geometry.PlacementOptions.Auto, OnPlacementOptionsChanged));

        protected static readonly DependencyProperty ConnectorVertexPointProperty = DependencyProperty.Register(
            "ConnectorVertexPoint",
            typeof(Point),
            typeof(BalloonBase),
            new PropertyMetadata(default(Point)));

        protected static readonly DependencyProperty ConnectorPoint1Property = DependencyProperty.Register(
            "ConnectorPoint1",
            typeof(Point),
            typeof(BalloonBase),
            new PropertyMetadata(default(Point)));

        protected static readonly DependencyProperty ConnectorPoint2Property = DependencyProperty.Register(
            "ConnectorPoint2",
            typeof(Point),
            typeof(BalloonBase),
            new PropertyMetadata(default(Point)));

        private readonly PenCache penCache = new PenCache();
        private Geometry balloonGeometry;
        private Geometry boxGeometry;

        static BalloonBase()
        {
            StretchProperty.OverrideMetadata(typeof(BalloonBase), new FrameworkPropertyMetadata(Stretch.Fill));
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

        protected Geometry ConnectorGeometry { get; private set; }

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
                this.ConnectorGeometry = Geometry.Empty;
                this.balloonGeometry = Geometry.Empty;
                return;
            }

            this.boxGeometry = this.CreateBoxGeometry(this.RenderSize);
            this.ConnectorGeometry = this.CreateConnectorGeometry(this.RenderSize);
            this.balloonGeometry = this.CreateGeometry(this.boxGeometry, this.ConnectorGeometry);
        }

        protected abstract Geometry CreateBoxGeometry(Size renderSize);

        protected abstract Geometry CreateConnectorGeometry(Size renderSize);

        protected virtual Geometry CreateGeometry(Geometry box, Geometry connector)
        {
            var ballonGeometry = new CombinedGeometry(GeometryCombineMode.Union, box, connector);
            //ballonGeometry.Freeze();
            return ballonGeometry;
        }

        protected virtual void UpdateConnectorOffset()
        {
            if (this.PlacementTarget != null && this.RenderSize.Width > 0)
            {
                if (this.IsVisible && this.PlacementTarget.IsVisible)
                {
                    var selfRect = new Rect(new Point(0, 0).ToScreen(this), this.RenderSize).ToScreen(this);
                    var targetRect = new Rect(new Point(0, 0).ToScreen(this.PlacementTarget), this.PlacementTarget.RenderSize).ToScreen(this);
                    var tp = this.PlacementOptions?.GetPointOnTarget(selfRect, targetRect);
                    if (tp == null)
                    {
                        this.InvalidateProperty(ConnectorOffsetProperty);
                        return;
                    }

                    var mp = selfRect.MidPoint();
                    var ip = new Line(mp, tp.Value).ClosestIntersection(selfRect);
                    Debug.Assert(ip != null, "Did not find an intersection, bug in the library");
                    if (ip == null)
                    {
                        // failing silently in release
                        this.InvalidateProperty(ConnectorOffsetProperty);
                    }

                    var v = tp.Value - ip.Value;
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (this.PlacementOptions != null && v.Length > 0 && this.PlacementOptions.Offset != 0)
                    {
                        v = v - this.PlacementOptions.Offset * v.Normalized();
                    }

                    this.SetCurrentValue(ConnectorOffsetProperty, v);
                }
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        protected virtual void OnLayoutUpdated(object _, EventArgs __)
        {
            this.UpdateConnectorOffset();
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (BalloonBase)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private static void OnConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (BalloonBase)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private static void OnPlacementOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (BalloonBase)d;
            balloon.OnLayoutUpdated(null, null);
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (BalloonBase)d;
            balloon.UpdateConnectorOffset();
            // unsubscribing and subscribing here to have only one subscription
            balloon.LayoutUpdated -= balloon.OnLayoutUpdated;
            balloon.LayoutUpdated += balloon.OnLayoutUpdated;
            var oldTarget = e.OldValue as UIElement;
            if (oldTarget != null)
            {
                WeakEventManager<UIElement, EventArgs>.RemoveHandler(oldTarget, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
            }

            var newTarget = e.NewValue as UIElement;
            if (newTarget != null)
            {
                WeakEventManager<UIElement, EventArgs>.AddHandler(newTarget, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
            }
        }

        private class PenCache
        {
            private Brush cachedBrush;
            private double cachedStrokeThickness;
            private Pen pen;

            public Pen GetPen(Brush brush, double strokeThickness)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Equals(this.cachedBrush, brush) && this.cachedStrokeThickness == strokeThickness)
                {
                    return this.pen;
                }

                this.cachedBrush = brush;
                this.cachedStrokeThickness = strokeThickness;
                this.pen = new Pen(brush, strokeThickness);
                return this.pen;
            }
        }
    }
}