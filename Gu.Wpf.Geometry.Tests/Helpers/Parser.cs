namespace Gu.Wpf.Geometry.Tests
{
    using System;
    using System.Globalization;
    using System.Windows;

    using Gu.Wpf.Geometry;

    public static class Parser
    {
        internal static Line AsLine(this string s)
        {
            var strings = s.Split(';');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }
            var sp = Point.Parse(strings[0]);
            var ep = Point.Parse(strings[1]);
            return new Line(sp, ep);
        }

        private static double AsDouble(this string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
