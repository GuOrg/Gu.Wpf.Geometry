namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    public class BoxBalloon : BalloonBase
    {
        protected override Geometry CreateBoxGeometry(Size renderSize)
        {
            var width = renderSize.Width - this.StrokeThickness;
            var height = renderSize.Height - this.StrokeThickness;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var cr = this.AdjustedCornerRadius();
                var p = cr.TopLeft > 0
                    ? new Point(cr.TopLeft + this.StrokeThickness / 2, this.StrokeThickness / 2)
                    : new Point(this.StrokeThickness / 2, this.StrokeThickness / 2);
                context.BeginFigure(p, true, true);
                p = p.WithOffset(width - cr.TopLeft - cr.TopRight, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, cr.TopRight, cr.TopRight);

                p = p.WithOffset(0, height - cr.TopRight - cr.BottomRight);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -cr.BottomRight, cr.BottomRight);

                p = p.WithOffset(-width + cr.BottomRight + cr.BottomLeft, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -cr.BottomLeft, -cr.BottomLeft);

                p = p.WithOffset(0, -height + cr.TopLeft + cr.BottomLeft);
                context.LineTo(p, true, true);
                context.DrawCorner(p, cr.TopLeft, -cr.TopLeft);
            }

            geometry.Freeze();
            return geometry;
        }

        protected override Geometry CreateConnectorGeometry(Size renderSize)
        {
            if (this.ConnectorOffset == default(Vector) || renderSize.IsEmpty)
            {
                return Geometry.Empty;
            }

            var rectangle = new Rect(new Point(0, 0), renderSize);
            rectangle.Inflate(-this.StrokeThickness, -this.StrokeThickness);
            if (rectangle.IsEmpty)
            {
                return Geometry.Empty;
            }

            var width = renderSize.Width - this.StrokeThickness;
            var height = renderSize.Height - this.StrokeThickness;
            var mp = new Point(width / 2, height / 2);
            var direction = this.ConnectorOffset.Normalized();
            var length = width * width + height * height;
            var line = new Line(mp, mp + length * direction);

            var ip = line.ClosestIntersection(rectangle);
            if (ip == null)
            {
                Debug.Assert(false, $"Line {line} does not intersect rectangle {rectangle}");
                // ReSharper disable once HeuristicUnreachableCode
                return Geometry.Empty;
            }

            var cr = this.AdjustedCornerRadius();
            var vertexPoint = ip.Value + this.ConnectorOffset;
            line = new Line(vertexPoint, mp + length * direction.Negated());
            var p1 = ConnectorPoint.Find(line, this.ConnectorAngle / 2, rectangle, cr);
            var p2 = ConnectorPoint.Find(line, -this.ConnectorAngle / 2, rectangle, cr);
            this.SetValue(ConnectorVertexPointProperty, vertexPoint);
            this.SetValue(ConnectorPoint1Property, p1);
            this.SetValue(ConnectorPoint2Property, p2);
            if (this.ConnectorGeometry == null || this.ConnectorGeometry.IsEmpty())
            {
                var figure = new PathFigure { IsClosed = true };
                figure.Bind(PathFigure.StartPointProperty)
                      .OneWayTo(this, ConnectorPoint1Property);
                var s1 = new LineSegment { IsStroked = true };
                s1.Bind(LineSegment.PointProperty)
                  .OneWayTo(this, ConnectorVertexPointProperty);

                var s2 = new LineSegment { IsStroked = true };
                s2.Bind(LineSegment.PointProperty)
                  .OneWayTo(this, ConnectorPoint2Property);
                figure.Segments.Add(s1);
                figure.Segments.Add(s2);
                var geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                return geometry;
            }

            return this.ConnectorGeometry;
        }

        protected virtual CornerRadius AdjustedCornerRadius()
        {
            var cr = this.CornerRadius.InflateBy(-this.StrokeThickness / 2)
                                 .WithMin(0);
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

            var factor = Math.Min(Math.Min(this.ActualWidth / top, this.ActualWidth / bottom),
                                  Math.Min(this.ActualHeight / left, this.ActualHeight / right));
            return cr.ScaleBy(factor)
                     .InflateBy(-this.StrokeThickness / 2)
                     .WithMin(0);
        }


        private static class ConnectorPoint
        {
            internal static Point Find(Line line, double angle, Rect rectangle, CornerRadius cornerRadius)
            {
                var rotated = line.RotateAroundStartPoint(angle);
                return FindForRotated(rotated, rectangle, cornerRadius);
            }

            private static Point FindForRotated(Line line, Rect rectangle, CornerRadius cornerRadius)
            {
                var ip = line.ClosestIntersection(rectangle);
                if (ip == null)
                {
                    return FindTangentPoint(line, rectangle, cornerRadius);
                }

                Circle corner;
                if (TryGetCorner(ip.Value, rectangle, cornerRadius, out corner))
                {
                    ip = corner.ClosestIntersection(line);
                    if (ip == null)
                    {
                        return FindTangentPoint(line, rectangle, cornerRadius);
                    }

                    return ip.Value;
                }

                return ip.Value;
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

                corner = default(Circle);
                return false;
            }

            private static Point FindTangentPoint(Line line, Rect rectangle, CornerRadius cornerRadius)
            {
                Circle corner;
                var toMid = line.PerpendicularLineTo(rectangle.MidPoint());
                Debug.Assert(toMid != null, "Cannot find tangent if line goes through center");
                if (toMid == null)
                {
                    // failing silently in release
                    return rectangle.MidPoint();
                }

                //Debug.Assert(!rectangle.Contains(toMid.Value.StartPoint), "Cannot find tangent if line intersects rectangle");
                if (toMid.Value.Direction.Axis() != null)
                {
                    return line.StartPoint.Closest(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomRight, rectangle.BottomLeft);
                }

                switch (toMid.Value.Direction.Quadrant())
                {
                    case Quadrant.NegativeXPositiveY:
                        corner = CreateTopRight(rectangle.TopRight, cornerRadius.TopRight);
                        break;
                    case Quadrant.PositiveXPositiveY:
                        corner = CreateTopLeft(rectangle.TopLeft, cornerRadius.TopLeft);
                        break;
                    case Quadrant.PositiveXNegativeY:
                        corner = CreateBottomLeft(rectangle.BottomLeft, cornerRadius.BottomLeft);
                        break;
                    case Quadrant.NegativeXNegativeY:
                        corner = CreateBottomRight(rectangle.BottomRight, cornerRadius.BottomRight);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (corner.Radius == 0)
                {
                    return corner.Center;
                }

                var toCenterDirection = line.StartPoint.VectorTo(corner.Center)
                                            .Normalized();
                var perpDirection = line.Direction.AngleTo(toCenterDirection) > 0
                                        ? toCenterDirection.Rotate(-90)
                                        : toCenterDirection.Rotate(90);
                var perpOffset = corner.Radius * perpDirection;
                var tangentPoint = corner.Center + perpOffset;
                return tangentPoint;
            }

            private static Circle CreateTopLeft(Point p, double r) => new Circle(p.WithOffset(r, r), r);

            private static Circle CreateTopRight(Point p, double r) => new Circle(p.WithOffset(-r, r), r);

            private static Circle CreateBottomRight(Point p, double r) => new Circle(p.WithOffset(-r, -r), r);

            private static Circle CreateBottomLeft(Point p, double r) => new Circle(p.WithOffset(r, -r), r);
        }
    }
}