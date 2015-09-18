namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    public static class PointExt
    {
        internal static Point Offset(this Point p, Vector direction, double distance)
        {
            return p + distance * direction;
        }

        internal static double DistanceTo(this Point p, Point other)
        {
            return (p - other).Length;
        }

        internal static Point Round(this Point p)
        {
            return new Point(Math.Round(p.X, 0), Math.Round(p.Y, 0));
        }

        internal static string ToDebugString(this Point p, string format = "F1")
        {
            return $"{p.X.ToString(format, CultureInfo.InvariantCulture)},{p.Y.ToString(format, CultureInfo.InvariantCulture)}";
        }
    }
}
