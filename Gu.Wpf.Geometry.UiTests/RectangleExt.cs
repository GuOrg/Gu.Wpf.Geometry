namespace Gu.Wpf.Geometry.UiTests
{
    using FlaUI.Core.Shapes;
    using Point = System.Windows.Point;

    public static class RectangleExt
    {
        public static Point TopLeft(this Rectangle rect) => new Point(rect.X, rect.Y);
    }
}