namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    public class BoxBalloon : Balloon
    {
        protected override void UpdateConnectorOffset()
        {
            if (this.PlacementTarget != null && this.RenderSize.Width > 0)
            {
                if (this.IsVisible && this.PlacementTarget.IsVisible)
                {
                    var rect = new Rect(new Point(0, 0), this.RenderSize).ToScreen(this);
                    var placementRect = new Rect(new Point(0, 0), this.PlacementTarget.RenderSize);
                    var tp = this.PlacementOptions?.GetPoint(placementRect) ?? new Point(0, 0);
                    tp = this.PlacementTarget.PointToScreen(tp);
                    if (rect.Contains(tp))
                    {
                        this.SetCurrentValue(ConnectorOffsetProperty, new Vector(0, 0));
                        return;
                    }

                    var mp = rect.MidPoint();
                    var ip = new Line(mp, tp).ClosestIntersection(rect);
                    if (ip == null)
                    {
                        throw new InvalidOperationException("Did not find an intersection, bug in the library");
                    }

                    var v = tp - ip.Value;
                    if (this.PlacementOptions != null && this.PlacementOptions.Offset != 0)
                    {
                        var uv = v.Normalized();
                        var offset = Vector.Multiply(this.PlacementOptions.Offset, uv);
                        v = v + offset;
                    }

                    this.SetCurrentValue(ConnectorOffsetProperty, v);
                }
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        protected override Geometry CreateBoxGeometry(Size size)
        {
            var width = size.Width - this.StrokeThickness;
            var height = size.Height - this.StrokeThickness;
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
                p = context.DrawCorner(p, cr.TopLeft, -cr.TopLeft);
            }

            geometry.Freeze();
            return geometry;
        }

        protected override Geometry CreateConnectorGeometry(Size size)
        {
            if (this.ConnectorOffset == default(Vector) || size.IsEmpty)
            {
                return Geometry.Empty;
            }

            var rectangle = new Rect(new Point(0, 0), size);
            rectangle.Inflate(-this.StrokeThickness, -this.StrokeThickness);
            if (rectangle.IsEmpty)
            {
                return Geometry.Empty;
            }

            var width = size.Width - this.StrokeThickness;
            var height = size.Height - this.StrokeThickness;
            var mp = new Point(width / 2, height / 2);
            var direction = this.ConnectorOffset.Normalized();
            var length = width * width + height * height;
            var line = new Line(mp, mp + length * direction);

            var ip = line.ClosestIntersection(rectangle);
            if (ip == null)
            {
                Debug.Assert(false, $"Line {line} does not intersect rectangle {rectangle}");
                return Geometry.Empty;
            }

            var cr = this.AdjustedCornerRadius();
            var sp = ip.Value + this.ConnectorOffset;
            line = new Line(sp, mp + length * direction.Negated());
            var p1 = ConnectorPoint.Find(line, this.ConnectorAngle / 2, rectangle, cr);
            var p2 = ConnectorPoint.Find(line, -this.ConnectorAngle / 2, rectangle, cr);

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(sp, true, true);
                context.LineTo(p1, true, true);
                context.LineTo(p2, true, true);
            }

            geometry.Freeze();
            return geometry;
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

                //Debug.Assert(!rectangle.Contains(toMid.Value.StartPoint), "Cannot find tangent if line intersects rectangle");
                if (toMid.Value.Direction.Axis() != null)
                {
                    return line.StartPoint.Closest(rectangle.TopLeft, rectangle.TopRight, rectangle.BottomRight, rectangle.BottomLeft);
                }

                switch (toMid.Value.Direction.Quadrant())
                {
                    case Quadrant.TopLeft:
                        corner = CreateTopRight(rectangle.TopRight, cornerRadius.TopRight);
                        break;
                    case Quadrant.TopRight:
                        corner = CreateTopLeft(rectangle.TopLeft, cornerRadius.TopLeft);
                        break;
                    case Quadrant.BottomRight:
                        corner = CreateBottomLeft(rectangle.BottomLeft, cornerRadius.BottomLeft);
                        break;
                    case Quadrant.BottomLeft:
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