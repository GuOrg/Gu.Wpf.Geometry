namespace Gu.Wpf.Geometry;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

/// <summary>
/// Base class for balloon shapes.
/// </summary>
public abstract class BalloonBase : Shape
{
    /// <summary>Identifies the <see cref="ConnectorOffset"/> dependency property.</summary>
    public static readonly DependencyProperty ConnectorOffsetProperty = DependencyProperty.Register(
        nameof(ConnectorOffset),
        typeof(Vector),
        typeof(BalloonBase),
        new FrameworkPropertyMetadata(
            default(Vector),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnConnectorChanged));

    /// <summary>Identifies the <see cref="ConnectorAngle"/> dependency property.</summary>
    public static readonly DependencyProperty ConnectorAngleProperty = DependencyProperty.Register(
        nameof(ConnectorAngle),
        typeof(double),
        typeof(BalloonBase),
        new FrameworkPropertyMetadata(
            15.0,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnConnectorChanged));

    /// <summary>Identifies the <see cref="PlacementTarget"/> dependency property.</summary>
    public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
        nameof(PlacementTarget),
        typeof(UIElement),
        typeof(BalloonBase),
        new PropertyMetadata(
            default(UIElement),
            OnPlacementTargetChanged));

    /// <summary>Identifies the <see cref="PlacementRectangle"/> dependency property.</summary>
    public static readonly DependencyProperty PlacementRectangleProperty = DependencyProperty.Register(
        nameof(PlacementRectangle),
        typeof(Rect),
        typeof(BalloonBase),
        new PropertyMetadata(
            Rect.Empty,
            (d, e) => ((BalloonBase)d).UpdateConnectorOffset()));

    /// <summary>Identifies the <see cref="PlacementOptions"/> dependency property.</summary>
    public static readonly DependencyProperty PlacementOptionsProperty = DependencyProperty.Register(
        nameof(PlacementOptions),
        typeof(PlacementOptions),
        typeof(BalloonBase),
        new PropertyMetadata(
            PlacementOptions.Auto,
            (d, e) => ((BalloonBase)d).OnLayoutUpdated(null, EventArgs.Empty)));

    /// <summary>Identifies the ConnectorVertexPoint dependency property.</summary>
    protected static readonly DependencyProperty ConnectorVertexPointProperty = DependencyProperty.Register(
        "ConnectorVertexPoint",
        typeof(Point),
        typeof(BalloonBase),
        new PropertyMetadata(default(Point)));

    /// <summary>Identifies the ConnectorPoint1 dependency property.</summary>
    protected static readonly DependencyProperty ConnectorPoint1Property = DependencyProperty.Register(
        "ConnectorPoint1",
        typeof(Point),
        typeof(BalloonBase),
        new PropertyMetadata(default(Point)));

    /// <summary>Identifies the ConnectorPoint2 dependency property.</summary>
    protected static readonly DependencyProperty ConnectorPoint2Property = DependencyProperty.Register(
        "ConnectorPoint2",
        typeof(Point),
        typeof(BalloonBase),
        new PropertyMetadata(default(Point)));

    private readonly PenCache penCache = new();
    private Geometry? balloonGeometry;

    static BalloonBase()
    {
        StretchProperty.OverrideMetadata(typeof(BalloonBase), new FrameworkPropertyMetadata(Stretch.Fill));
    }

    /// <summary>
    /// Gets or sets the <see cref="Vector"/> specifying the connector of the <see cref="BalloonBase"/>.
    /// </summary>
    public Vector ConnectorOffset
    {
        get => (Vector)this.GetValue(ConnectorOffsetProperty);
        set => this.SetValue(ConnectorOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the angle of the connector of the <see cref="BalloonBase"/>.
    /// </summary>
    public double ConnectorAngle
    {
        get => (double)this.GetValue(ConnectorAngleProperty);
        set => this.SetValue(ConnectorAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets PlacementTarget property of the <see cref="BalloonBase"/>.
    /// </summary>
    public UIElement? PlacementTarget
    {
        get => (UIElement?)this.GetValue(PlacementTargetProperty);
        set => this.SetValue(PlacementTargetProperty, value);
    }

    /// <summary>
    /// Gets or sets PlacementRectangle property of the balloon.
    /// </summary>
    public Rect PlacementRectangle
    {
        get => (Rect)this.GetValue(PlacementRectangleProperty);
        set => this.SetValue(PlacementRectangleProperty, value);
    }

    /// <summary>
    /// Gets or sets <see cref="PlacementOptions"/> property of the <see cref="BalloonBase"/>.
    /// </summary>
    public PlacementOptions PlacementOptions
    {
        get => (PlacementOptions)this.GetValue(PlacementOptionsProperty);
        set => this.SetValue(PlacementOptionsProperty, value);
    }

    /// <summary>
    /// Gets the <see cref="Geometry"/> that defines the balloon.
    /// </summary>
    protected override Geometry DefiningGeometry => this.BoxGeometry ?? Geometry.Empty;

    /// <summary>
    /// Gets the <see cref="Geometry"/> that defines the connector.
    /// </summary>
    protected Geometry? ConnectorGeometry { get; private set; }

    /// <summary>
    /// Gets the <see cref="Geometry"/> that defines the box.
    /// </summary>
    protected Geometry? BoxGeometry { get; private set; }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        return new Size(this.StrokeThickness, this.StrokeThickness);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (finalSize.Width > this.StrokeThickness && finalSize.Height > this.StrokeThickness)
        {
            finalSize = new Size(finalSize.Width - this.StrokeThickness, finalSize.Height - this.StrokeThickness);
        }

        return finalSize;
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (drawingContext is null)
        {
            throw new ArgumentNullException(nameof(drawingContext));
        }

        var pen = this.penCache.GetPen(this.Stroke, this.StrokeThickness);
        drawingContext.DrawGeometry(this.Fill, pen, this.balloonGeometry);
    }

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        this.UpdateConnectorOffset();
        this.UpdateCachedGeometries();
        this.InvalidateVisual();
    }

    /// <summary>
    /// Updates <see cref="BoxGeometry"/> and <see cref="ConnectorGeometry"/>.
    /// </summary>
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

    /// <summary>
    /// Check if connector c an be created.
    /// </summary>
    /// <returns>True if conditions are satisfied.</returns>
    protected bool CanCreateConnectorGeometry()
    {
        return this.ConnectorOffset != default &&
               this.RenderSize.Width > 0 &&
               this.RenderSize.Height > 0;
    }

    /// <summary>
    /// Get or create the box geometry.
    /// </summary>
    /// <param name="renderSize">The <see cref="Size"/>.</param>
    /// <returns>The <see cref="Geometry"/>.</returns>
    protected abstract Geometry GetOrCreateBoxGeometry(Size renderSize);

    /// <summary>
    /// Get or create the connector geometry.
    /// </summary>
    /// <param name="renderSize">The <see cref="Size"/>.</param>
    /// <returns>The <see cref="Geometry"/>.</returns>
    protected abstract Geometry GetOrCreateConnectorGeometry(Size renderSize);

    /// <summary>
    /// Get or create the geometry.
    /// </summary>
    /// <param name="box">The box <see cref="Geometry"/>.</param>
    /// <param name="connector">The connector <see cref="Geometry"/>.</param>
    /// <returns>The <see cref="Geometry"/>.</returns>
    protected virtual Geometry CreateGeometry(Geometry box, Geometry connector)
    {
        return new CombinedGeometry(GeometryCombineMode.Union, box, connector);
    }

    /// <summary>
    /// Update the connector offset.
    /// </summary>
    protected virtual void UpdateConnectorOffset()
    {
        var hasTarget =
            this.PlacementTarget?.IsVisible == true ||
            !this.PlacementRectangle.IsEmpty;

        if (this.IsVisible && this.RenderSize.Width > 0 && hasTarget)
        {
            if (!this.IsLoaded)
            {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                _ = this.Dispatcher.BeginInvoke(new Action(this.UpdateConnectorOffset), DispatcherPriority.Loaded);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
                return;
            }

            var selfRect = new Rect(new Point(0, 0).ToScreen(this), this.RenderSize).ToScreen(this);
            var targetRect = this.GetTargetRect();

            var tp = this.PlacementOptions?.GetPointOnTarget(selfRect, targetRect);
            if (tp is null || selfRect.Contains(tp.Value))
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
                return;
            }

            var mp = selfRect.CenterPoint();
            var ip = new Line(mp, tp.Value).ClosestIntersection(selfRect);

            if (ip is null)
            {
                Debug.Fail("Did not find an intersection, bug in the library");

                // failing silently in release
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
            else
            {
                var v = tp.Value - ip.Value;
                //// ReSharper disable once CompareOfFloatsByEqualityOperator
                if (this.PlacementOptions != null && v.Length > 0 && this.PlacementOptions.Offset != 0)
                {
                    v -= this.PlacementOptions.Offset * v.Normalized();
                }

                this.SetCurrentValue(ConnectorOffsetProperty, v);
            }
        }
        else
        {
            this.InvalidateProperty(ConnectorOffsetProperty);
        }
    }

    /// <summary>
    /// Called when properties causing layout update changes.
    /// </summary>
    /// <param name="sender">The <see cref="BalloonBase"/> that the change happened on.</param>
    /// <param name="e">The <see cref="EventArgs"/>.</param>
#pragma warning disable CA2109 // Review visible event handlers
    protected virtual void OnLayoutUpdated(object? sender, EventArgs e)
#pragma warning restore CA2109 // Review visible event handlers
    {
        this.UpdateConnectorOffset();
    }

    /// <summary>
    /// Gets <see cref="PlacementTarget"/> if set or GetVisualParent().
    /// </summary>
    /// <returns>The target <see cref="UIElement"/>.</returns>
    protected virtual UIElement? GetTarget()
    {
        return this.PlacementTarget ??
               this.GetVisualParent();
    }

    /// <summary>
    /// Get the target <see cref="Rect"/>.
    /// </summary>
    /// <returns>The <see cref="Rect"/>.</returns>
    protected virtual Rect GetTargetRect()
    {
        var targetRect = Rect.Empty;

        if (this.PlacementRectangle.IsEmpty &&
            this.PlacementTarget is { } placementTarget)
        {
            targetRect = new Rect(new Point(0, 0).ToScreen(placementTarget), placementTarget.RenderSize).ToScreen(this);
        }
        else
        {
            var target = this.GetTarget();
            if (target != null)
            {
                targetRect = this.PlacementRectangle.ToScreen(target).ToScreen(this);
            }
        }

        return targetRect;
    }

    private static void OnConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var balloon = (BalloonBase)d;
        if (balloon.IsInitialized)
        {
            balloon.UpdateCachedGeometries();
        }
    }

    private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var balloon = (BalloonBase)d;
        balloon.UpdateConnectorOffset();

        // unsubscribing and subscribing here to have only one subscription
        balloon.LayoutUpdated -= balloon.OnLayoutUpdated;
        balloon.LayoutUpdated += balloon.OnLayoutUpdated;
        if (e.OldValue is UIElement oldTarget)
        {
            WeakEventManager<UIElement, EventArgs>.RemoveHandler(oldTarget, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
        }

        if (e.NewValue is UIElement newTarget)
        {
            WeakEventManager<UIElement, EventArgs>.AddHandler(newTarget, nameof(LayoutUpdated), balloon.OnLayoutUpdated);
        }
    }

    private class PenCache
    {
        private Brush? cachedBrush;
        private double cachedStrokeThickness;
        private Pen? pen;

        internal Pen GetPen(Brush brush, double strokeThickness)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Equals(this.cachedBrush, brush) && this.cachedStrokeThickness == strokeThickness)
            {
                return this.pen ?? throw new InvalidOperationException("Failed getting pen.");
            }

            this.cachedBrush = brush;
            this.cachedStrokeThickness = strokeThickness;
            this.pen = new Pen(brush, strokeThickness);
            return this.pen;
        }
    }
}
