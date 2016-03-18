namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    internal static  class StreamGeometryContextExt
    {
        internal static Point DrawCorner(this StreamGeometryContext context, Point fromPoint, double offsetX, double offsetY)
        {
            if (offsetX == 0)
            {
                return fromPoint;
            }

            var p = fromPoint.WithOffset(offsetX, offsetY);
            var size = new Size(Math.Abs(offsetX), Math.Abs(offsetY));
            context.ArcTo(p, size, 90, false, SweepDirection.Clockwise, true, true);
            return p;
        }
    }
}