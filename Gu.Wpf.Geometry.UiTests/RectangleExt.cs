namespace Gu.Wpf.Geometry.UiTests
{
    using System.Windows;
    using Point = System.Windows.Point;

    public static class RectangleExt
    {
        public static Point TopLeft(this Rect rect) => new Point(rect.X, rect.Y);
    }
}