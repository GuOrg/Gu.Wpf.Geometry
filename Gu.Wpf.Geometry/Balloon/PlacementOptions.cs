namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    [TypeConverter(typeof(PlacementOptionsConverter))]
    public sealed class PlacementOptions
    {
        public static readonly PlacementOptions Auto = new PlacementOptions(HorizontalPlacement.Auto, VerticalPlacement.Auto, 0);
        public static readonly PlacementOptions Center = new PlacementOptions(HorizontalPlacement.Center, VerticalPlacement.Center, 0);

        public PlacementOptions(HorizontalPlacement horizontal, VerticalPlacement vertical, double offset)
        {
            this.Vertical = vertical;
            this.Horizontal = horizontal;
            this.Offset = offset;
        }

        public HorizontalPlacement Horizontal { get; }

        public VerticalPlacement Vertical { get; }

        public double Offset { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Horizontal} {this.Vertical} {this.Offset}";
        }

        public Point? GetPointOnTarget(Rect placed, Rect target)
        {
            var p = this.GetPointOnTarget(placed.CenterPoint(), target);

            return p;
        }

        private static Line ClosestLine(Rect rect, Point p)
        {
            var angle = new Vector(1, 1).AngleTo(p.VectorTo(rect.CenterPoint()));
            if (angle >= 0 && angle <= 90)
            {
                return rect.TopLine();
            }

            if (angle >= 90 && angle <= 180)
            {
                return rect.RightLine();
            }

            if (angle >= -90 && angle <= 0)
            {
                return rect.LeftLine();
            }

            if (angle >= -180 && angle <= -90)
            {
                return rect.BottomLine();
            }

            throw new InvalidOperationException("Could not find closest line");
        }

        private static Point? AutoPoint(Line toMid, Line edge)
        {
            var angleTo = toMid.Direction.AngleTo(edge.Direction);
            if (angleTo < 0)
            {
                return toMid.IntersectWith(edge, mustBeBetweenStartAndEnd: false)
                    ?.TrimToLine(edge);
            }

            return toMid.StartPoint.Closest(edge.StartPoint, edge.EndPoint);
        }

        private Point? GetPointOnTarget(Point sourceMidPoint, Rect target)
        {
            return this.Vertical switch
            {
                VerticalPlacement.Auto => this.Horizontal switch
                {
                    HorizontalPlacement.Auto => AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), ClosestLine(target, sourceMidPoint)),
                    HorizontalPlacement.Left => AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.LeftLine()),
                    HorizontalPlacement.Center => sourceMidPoint.Closest(target.BottomLine(), target.TopLine()).MidPoint,
                    HorizontalPlacement.Right => AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.RightLine()),
                    _ => throw new InvalidEnumArgumentException("Unhandled HorizontalPlacement."),
                },
                VerticalPlacement.Top => this.Horizontal switch
                {
                    HorizontalPlacement.Auto => AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.TopLine()),
                    HorizontalPlacement.Left => target.TopLeft,
                    HorizontalPlacement.Center => PointExt.MidPoint(target.TopLeft, target.TopRight),
                    HorizontalPlacement.Right => target.TopRight,
                    _ => throw new InvalidEnumArgumentException("Unhandled HorizontalPlacement."),
                },
                VerticalPlacement.Center => this.Horizontal switch
                {
                    HorizontalPlacement.Auto => sourceMidPoint.Closest(target.LeftLine(), target.RightLine()).MidPoint,
                    HorizontalPlacement.Left => PointExt.MidPoint(target.BottomLeft, target.TopLeft),
                    HorizontalPlacement.Center => PointExt.MidPoint(target.TopLeft, target.BottomRight),
                    HorizontalPlacement.Right => PointExt.MidPoint(target.BottomRight, target.TopRight),
                    _ => throw new InvalidEnumArgumentException("Unhandled HorizontalPlacement."),
                },
                VerticalPlacement.Bottom => this.Horizontal switch
                {
                    HorizontalPlacement.Auto => AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.BottomLine()),
                    HorizontalPlacement.Left => target.BottomLeft,
                    HorizontalPlacement.Center => PointExt.MidPoint(target.BottomLeft, target.BottomRight),
                    HorizontalPlacement.Right => target.BottomRight,
                    _ => throw new InvalidEnumArgumentException("Unhandled HorizontalPlacement."),
                },
                _ => throw new InvalidEnumArgumentException("Unhandled VerticalPlacement."),
            };
        }
    }
}
