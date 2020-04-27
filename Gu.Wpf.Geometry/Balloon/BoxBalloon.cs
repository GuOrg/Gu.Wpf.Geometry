namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    public class BoxBalloon : BalloonBase
    {
        /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(BoxBalloon),
            new FrameworkPropertyMetadata(
                default(CornerRadius),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnCornerRadiusChanged));

        private static readonly DependencyProperty RectProperty = DependencyProperty.Register(
            "Rect",
            typeof(Rect),
            typeof(BoxBalloon),
            new PropertyMetadata(Rect.Empty));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }

        protected override Geometry GetOrCreateBoxGeometry(Size renderSize)
        {
            var rect = new Rect(new Point(0, 0), renderSize);
            this.SetCurrentValue(RectProperty, rect);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return Geometry.Empty;
            }

            if (this.CornerRadius.IsAllEqual())
            {
                // using TopLeft here as we have already checked that they are equal
                if (this.BoxGeometry is RectangleGeometry)
                {
                    return this.BoxGeometry;
                }

                var geometry = new RectangleGeometry();
                _ = geometry.Bind(RectangleGeometry.RectProperty)
                            .OneWayTo(this, RectProperty);
                _ = geometry.Bind(RectangleGeometry.RadiusXProperty)
                            .OneWayTo(this, CornerRadiusProperty, CornerRadiusTopLeftConverter.Default);
                _ = geometry.Bind(RectangleGeometry.RadiusYProperty)
                            .OneWayTo(this, CornerRadiusProperty, CornerRadiusTopLeftConverter.Default);
                return geometry;
            }
            else
            {
                var geometry = new StreamGeometry();
                using (var context = geometry.Open())
                {
                    var cr = this.AdjustedCornerRadius();
                    var p = cr.TopLeft > 0
                        ? new Point(cr.TopLeft + this.StrokeThickness / 2, this.StrokeThickness / 2)
                        : new Point(this.StrokeThickness / 2, this.StrokeThickness / 2);
                    context.BeginFigure(p, isFilled: true, isClosed: true);
                    p = p.WithOffset(rect.Width - cr.TopLeft - cr.TopRight, 0);
                    context.LineTo(p, isStroked: true, isSmoothJoin: true);
                    p = context.DrawCorner(p, cr.TopRight, cr.TopRight);

                    p = p.WithOffset(0, rect.Height - cr.TopRight - cr.BottomRight);
                    context.LineTo(p, isStroked: true, isSmoothJoin: true);
                    p = context.DrawCorner(p, -cr.BottomRight, cr.BottomRight);

                    p = p.WithOffset(-rect.Width + cr.BottomRight + cr.BottomLeft, 0);
                    context.LineTo(p, isStroked: true, isSmoothJoin: true);
                    p = context.DrawCorner(p, -cr.BottomLeft, -cr.BottomLeft);

                    p = p.WithOffset(0, -rect.Height + cr.TopLeft + cr.BottomLeft);
                    context.LineTo(p, isStroked: true, isSmoothJoin: true);
                    _ = context.DrawCorner(p, cr.TopLeft, -cr.TopLeft);
                }

                geometry.Freeze();
                return geometry;
            }
        }

        protected override Geometry GetOrCreateConnectorGeometry(Size renderSize)
        {
            var rectangle = new Rect(new Point(0, 0), renderSize);
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
            {
                return Geometry.Empty;
            }

            var fromCenter = new Ray(rectangle.CenterPoint(), this.ConnectorOffset);
            var ip = fromCenter.FirstIntersectionWith(rectangle);
            if (ip is null)
            {
                Debug.Assert(condition: false, message: $"Line {fromCenter} does not intersect rectangle {rectangle}");

                // ReSharper disable once HeuristicUnreachableCode
                return Geometry.Empty;
            }

            var cr = this.AdjustedCornerRadius();
            var vertexPoint = ip.Value + this.ConnectorOffset;
            var toCenter = new Ray(vertexPoint, this.ConnectorOffset.Negated());
            var p1 = ConnectorPoint.Find(toCenter, this.ConnectorAngle / 2, this.StrokeThickness, rectangle, cr);
            var p2 = ConnectorPoint.Find(toCenter, -this.ConnectorAngle / 2, this.StrokeThickness, rectangle, cr);
            this.SetCurrentValue(ConnectorVertexPointProperty, vertexPoint);
            this.SetCurrentValue(ConnectorPoint1Property, p1);
            this.SetCurrentValue(ConnectorPoint2Property, p2);
            if (this.ConnectorGeometry is PathGeometry)
            {
                return this.ConnectorGeometry;
            }

            var figure = this.CreatePathFigureStartingAt(ConnectorPoint1Property);
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorVertexPointProperty));
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorPoint2Property));
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        protected virtual CornerRadius AdjustedCornerRadius()
        {
            var cr = this.CornerRadius;
            var left = cr.TopLeft + cr.BottomLeft;
            var right = cr.TopRight + cr.BottomRight;
            var top = cr.TopLeft + cr.TopRight;
            var bottom = cr.BottomLeft + cr.BottomRight;
            if (left < this.ActualHeight &&
                right < this.ActualHeight &&
                top < this.ActualWidth &&
                bottom < this.ActualWidth)
            {
                return cr;
            }

            var factor = Math.Min(
                Math.Min(this.ActualWidth / top, this.ActualWidth / bottom),
                Math.Min(this.ActualHeight / left, this.ActualHeight / right));
            return cr.ScaleBy(factor);
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (BoxBalloon)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private PathFigure CreatePathFigureStartingAt(DependencyProperty property)
        {
            var figure = new PathFigure { IsClosed = true };
            _ = figure.Bind(PathFigure.StartPointProperty)
                      .OneWayTo(this, property);
            return figure;
        }

        private LineSegment CreateLineSegmentTo(DependencyProperty property)
        {
            var lineSegment = new LineSegment { IsStroked = true };
            _ = lineSegment.Bind(LineSegment.PointProperty)
                           .OneWayTo(this, property);
            return lineSegment;
        }

        private static class ConnectorPoint
        {
            internal static Point Find(Ray toCenter, double angle, double strokeThickness, Rect rectangle, CornerRadius cornerRadius)
            {
                var rotated = toCenter.Rotate(angle);
                return FindForRotated(rotated, strokeThickness, rectangle, cornerRadius);
            }

            private static Point FindForRotated(Ray ray, double strokeThickness, Rect rectangle, CornerRadius cornerRadius)
            {
                var ip = ray.FirstIntersectionWith(rectangle);
                if (ip is null)
                {
                    return FindTangentPoint(ray, rectangle, cornerRadius);
                }

                if (TryGetCorner(ip.Value, rectangle, cornerRadius, out var corner))
                {
                    ip = ray.FirstIntersectionWith(corner);
                    if (ip is null)
                    {
                        return FindTangentPoint(ray, rectangle, cornerRadius);
                    }

                    return ip.Value + strokeThickness * ray.Direction;
                }

                return ip.Value + strokeThickness * ray.Direction;
            }

            private static bool TryGetCorner(Point intersectionPoint, Rect rectangle, CornerRadius cornerRadius, out Circle corner)
            {
                return TryGetCorner(intersectionPoint, rectangle.TopLeft, cornerRadius.TopLeft, CreateTopLeft, out corner) ||
                       TryGetCorner(intersectionPoint, rectangle.TopRight, cornerRadius.TopRight, CreateTopRight, out corner) ||
                       TryGetCorner(intersectionPoint, rectangle.BottomRight, cornerRadius.BottomRight, CreateBottomRight, out corner) ||
                       TryGetCorner(intersectionPoint, rectangle.BottomLeft, cornerRadius.BottomLeft, CreateBottomLeft, out corner);
            }

            private static bool TryGetCorner(Point intersectionPoint, Point cornerPoint, double radius, Func<Point, double, Circle> factory, out Circle corner)
            {
                if (intersectionPoint.DistanceTo(cornerPoint) < radius)
                {
                    corner = factory(cornerPoint, radius);
                    return true;
                }

                corner = default;
                return false;
            }

            private static Point FindTangentPoint(Ray ray, Rect rectangle, CornerRadius cornerRadius)
            {
                var toMid = ray.PerpendicularLineTo(rectangle.CenterPoint());
                Debug.Assert(toMid != null, "Cannot find tangent if line goes through center");
                //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (toMid is null)
                {
                    // failing silently in release
                    return rectangle.CenterPoint();
                }

                // Debug.Assert(!rectangle.Contains(toMid.Value.StartPoint), "Cannot find tangent if line intersects rectangle");
                if (toMid.Value.Direction.Axis() != null)
                {
                    return ray.Point.Closest(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomRight, rectangle.BottomLeft);
                }

                var corner = toMid.Value.Direction.Quadrant() switch
                {
                    Quadrant.NegativeXPositiveY => CreateTopRight(rectangle.TopRight, cornerRadius.TopRight),
                    Quadrant.PositiveXPositiveY => CreateTopLeft(rectangle.TopLeft, cornerRadius.TopLeft),
                    Quadrant.PositiveXNegativeY => CreateBottomLeft(rectangle.BottomLeft, cornerRadius.BottomLeft),
                    Quadrant.NegativeXNegativeY => CreateBottomRight(rectangle.BottomRight, cornerRadius.BottomRight),
                    _ => throw new InvalidEnumArgumentException("Unhandled quadrant."),
                };

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (corner.Radius == 0)
                {
                    return corner.Center;
                }

                var lineToCenter = ray.PerpendicularLineTo(corner.Center);
                Debug.Assert(lineToCenter != null, "Ray cannot go through center here");
                //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (lineToCenter is null)
                {
                    // this should never happen but failing silently
                    // the balloons should not throw much.
                    return corner.Center;
                }

                return corner.Center - corner.Radius * lineToCenter.Value.Direction;
            }

            private static Circle CreateTopLeft(Point p, double r) => new Circle(p.WithOffset(r, r), r);

            private static Circle CreateTopRight(Point p, double r) => new Circle(p.WithOffset(-r, r), r);

            private static Circle CreateBottomRight(Point p, double r) => new Circle(p.WithOffset(-r, -r), r);

            private static Circle CreateBottomLeft(Point p, double r) => new Circle(p.WithOffset(r, -r), r);
        }
    }
}
