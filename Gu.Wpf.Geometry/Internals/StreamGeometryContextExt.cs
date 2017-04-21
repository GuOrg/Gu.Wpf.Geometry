namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    internal static class StreamGeometryContextExt
    {
        internal static Point DrawCorner(this StreamGeometryContext context, Point fromPoint, double offsetX, double offsetY)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offsetX == 0)
            {
                return fromPoint;
            }

            var p = fromPoint.WithOffset(offsetX, offsetY);
            var size = new Size(Math.Abs(offsetX), Math.Abs(offsetY));
            context.ArcTo(
                point: p,
                size: size,
                rotationAngle: 90,
                isLargeArc: false,
                sweepDirection: SweepDirection.Clockwise,
                isStroked: true,
                isSmoothJoin: true);
            return p;
        }
    }
}