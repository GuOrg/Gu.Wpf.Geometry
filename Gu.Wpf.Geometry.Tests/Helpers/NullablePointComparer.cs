namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class NullablePointComparer : IEqualityComparer<Point?>
    {
        private readonly int digits;
        public static readonly NullablePointComparer Default = new NullablePointComparer(2);

        public NullablePointComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Point? x, Point? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return PointComparer.Default.Equals(x.Value, y.Value, this.digits);
        }

        public int GetHashCode(Point? obj)
        {
            return obj?.Round(this.digits).GetHashCode() ?? 0;
        }
    }
}