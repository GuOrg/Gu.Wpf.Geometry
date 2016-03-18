namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    [TypeConverter(typeof(PlacementOptionsConverter))]
    public class PlacementOptions
    {
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

        public Point GetPoint(Rect rect)
        {
            switch (this.Vertical)
            {
                case VerticalPlacement.Top:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Left:
                            return rect.TopLeft;
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(rect.TopLeft, rect.TopRight);
                        case HorizontalPlacement.Right:
                            return rect.TopRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Center:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Left:
                            return PointExt.MidPoint(rect.BottomLeft, rect.TopLeft);
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(rect.TopLeft, rect.BottomRight);
                        case HorizontalPlacement.Right:
                            return PointExt.MidPoint(rect.BottomRight, rect.TopRight);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Bottom:
                    switch (this.Horizontal)
                    {
                        case HorizontalPlacement.Left:
                            return rect.BottomLeft;
                        case HorizontalPlacement.Center:
                            return PointExt.MidPoint(rect.BottomLeft, rect.BottomRight);
                        case HorizontalPlacement.Right:
                            return rect.BottomRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
