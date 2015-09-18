namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class PointComparer : IEqualityComparer<Point>
    {
        public static readonly PointComparer Default = new PointComparer();
        public bool Equals(Point x, Point y)
        {
            return x.Round() == y.Round();
        }

        public int GetHashCode(Point obj)
        {
            unchecked
            {
                return obj.Round().GetHashCode();
            }
        }
    }
}