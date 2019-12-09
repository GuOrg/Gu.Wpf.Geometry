namespace Gu.Wpf.Geometry.Tests
{
    using System;
    using System.Windows;

    using Gu.Wpf.Geometry;

    public static class Parser
    {
        internal static Line AsLine(this string s)
        {
            var strings = s.Split(';');
            if (strings.Length != 2)
            {
                throw new FormatException("Could not parse line.");
            }

            var sp = Point.Parse(strings[0]);
            var ep = Point.Parse(strings[1]);
            return new Line(sp, ep);
        }
    }
}
