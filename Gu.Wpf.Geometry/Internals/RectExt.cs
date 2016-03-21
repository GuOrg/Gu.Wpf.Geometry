namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;

    internal static class RectExt
    {
        internal static Line TopLine(this Rect rect)
        {
            return new Line(rect.TopLeft, rect.TopRight);
        }

        internal static Line BottomLine(this Rect rect)
        {
            return new Line(rect.BottomRight, rect.BottomLeft);
        }

        internal static Line LeftLine(this Rect rect)
        {
            return new Line(rect.BottomLeft, rect.TopLeft);
        }

        internal static Line RightLine(this Rect rect)
        {
            return new Line(rect.TopRight, rect.BottomRight);
        }

        internal static Point CenterPoint(this Rect rect)
        {
            return PointExt.MidPoint(rect.TopLeft, rect.BottomRight);
        }

        internal static Point ClosestCornerPoint(this Rect rect, Point p)
        {
            return p.Closest(rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft);
        }

        internal static Rect ToScreen(this Rect rect, UIElement element)
        {
            var topLeft = element.PointToScreen(rect.TopLeft);
            var bottomRight = element.PointToScreen(rect.BottomRight);
            return new Rect(topLeft, bottomRight);
        }
    }
}
