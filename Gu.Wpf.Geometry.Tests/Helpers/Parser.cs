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
            return new Line(strings[0].AsPoint(), strings[1].AsPoint());
        }

        public static Point AsPoint(this string s)
        {
            var strings = s.Split(',');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }
            return new Point(strings[0].AsDouble(), strings[1].AsDouble());
        }

        private static double AsDouble(this string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
