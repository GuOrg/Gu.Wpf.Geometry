namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    [TypeConverter(typeof(PlacementOptionsConverter))]
    public class PlacementOptions
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

        public override string ToString()
        {
            return $"{this.Horizontal} {this.Vertical} {this.Offset}";
        }

        public Point? GetPointOnTarget(Rect placed, Rect target)
        {
            var p = this.GetPointOnTarget(placed.CenterPoint(), target);
            if (p == null)
            {
                return null;
            }

            if (placed.Contains(p.Value))
            {
                return null;
            }

            return p;
        }

        private static Line ClosestLine(Rect rect, Point p)
        {
            var angle = new Vector(1, 1).AngleTo(p.VectorTo(rect.CenterPoint()));
            if (0 <= angle && angle <= 90)
            {
                return rect.TopLine();
            }

            if (90 <= angle && angle <= 180)
            {
                return rect.RightLine();
            }

            if (-90 <= angle && angle <= 0)
            {
                return rect.LeftLine();
            }

            if (-180 <= angle && angle <= -90)
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
                return toMid.IntersectWith(edge, false)
                    ?.TrimToLine(edge);
            }

            return toMid.StartPoint.Closest(edge.StartPoint, edge.EndPoint);
        }

        private Point? GetPointOnTarget(Point sourceMidPoint, Rect target)
        {
            switch (this.Vertical)
            {
                case VerticalPlacement.Auto:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Auto:
                            var closestLine = ClosestLine(target, sourceMidPoint);
                            return AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), closestLine);
                        case HorizontalPlacement.Left:
                            return AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.LeftLine());
                        case HorizontalPlacement.Center:
                            return sourceMidPoint.Closest(target.BottomLine(), target.TopLine())
                                                 .MidPoint;
                        case HorizontalPlacement.Right:
                            return AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.RightLine());
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Top:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Auto:
                            return AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.TopLine());
                        case HorizontalPlacement.Left:
                            return target.TopLeft;
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(target.TopLeft, target.TopRight);
                        case HorizontalPlacement.Right:
                            return target.TopRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Center:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Auto:
                            return sourceMidPoint.Closest(target.LeftLine(), target.RightLine())
                                                 .MidPoint;
                        case HorizontalPlacement.Left:
                            return PointExt.MidPoint(target.BottomLeft, target.TopLeft);
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(target.TopLeft, target.BottomRight);
                        case HorizontalPlacement.Right:
                            return PointExt.MidPoint(target.BottomRight, target.TopRight);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Bottom:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Auto:
                            return AutoPoint(sourceMidPoint.LineTo(target.CenterPoint()), target.BottomLine());
                        case HorizontalPlacement.Left:
                            return target.BottomLeft;
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(target.BottomLeft, target.BottomRight);
                        case HorizontalPlacement.Right:
                            return target.BottomRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
