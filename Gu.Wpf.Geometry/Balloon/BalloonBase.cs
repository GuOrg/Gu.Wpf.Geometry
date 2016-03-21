namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public abstract class BalloonBase : Shape
    {
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
            new PropertyMetadata(
                default(UIElement),
                OnPlacementTargetChanged));

        public static readonly DependencyProperty PlacementOptionsProperty = DependencyProperty.Register(
            "PlacementOptions",
            typeof(PlacementOptions),
            typeof(BalloonBase),
            new PropertyMetadata(
                PlacementOptions.Auto, 
                OnPlacementOptionsChanged));

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

        static BalloonBase()
        {
            StretchProperty.OverrideMetadata(typeof(BalloonBase), new FrameworkPropertyMetadata(Stretch.Fill));
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

        protected override Geometry DefiningGeometry => this.BoxGeometry ?? Geometry.Empty;

        protected Geometry ConnectorGeometry { get; private set; }

        protected Geometry BoxGeometry { get; private set; }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.StrokeThickness, this.StrokeThickness);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width > this.StrokeThickness && finalSize.Height > this.StrokeThickness)
            {
                finalSize = new Size(finalSize.Width - this.StrokeThickness, finalSize.Height - this.StrokeThickness);
            }

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
                if (this.BoxGeometry != null)
                {
                    BindingOperations.ClearAllBindings(this.BoxGeometry);
                }

                this.BoxGeometry = Geometry.Empty;
                if (this.ConnectorGeometry != null)
                {
                    BindingOperations.ClearAllBindings(this.ConnectorGeometry);
                }

                this.ConnectorGeometry = Geometry.Empty;
                this.balloonGeometry = Geometry.Empty;
                return;
            }

            var boxGeometry = this.GetOrCreateBoxGeometry(this.RenderSize);
            var connectorGeometry = this.CanCreateConnectorGeometry()
                    ? this.GetOrCreateConnectorGeometry(this.RenderSize)
                    : Geometry.Empty;
            if (ReferenceEquals(boxGeometry, this.BoxGeometry) &&
                ReferenceEquals(connectorGeometry, this.ConnectorGeometry))
            {
                return;
            }

            if (this.BoxGeometry != null && !ReferenceEquals(boxGeometry, this.BoxGeometry))
            {
                BindingOperations.ClearAllBindings(this.BoxGeometry);
            }

            this.BoxGeometry = boxGeometry;
            if (this.ConnectorGeometry != null && !ReferenceEquals(connectorGeometry, this.ConnectorGeometry))
            {
                BindingOperations.ClearAllBindings(this.ConnectorGeometry);
            }

            this.ConnectorGeometry = connectorGeometry;
            this.balloonGeometry = this.CreateGeometry(this.BoxGeometry, this.ConnectorGeometry);
        }

        private bool CanCreateConnectorGeometry()
        {
            return this.ConnectorOffset != default(Vector) &&
                   this.RenderSize.Width > 0 &&
                   this.RenderSize.Height > 0;
        }

        protected abstract Geometry GetOrCreateBoxGeometry(Size renderSize);

        protected abstract Geometry GetOrCreateConnectorGeometry(Size renderSize);

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

                    var mp = selfRect.CenterPoint();
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