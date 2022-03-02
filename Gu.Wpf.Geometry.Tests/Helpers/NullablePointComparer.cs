namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public sealed class NullablePointComparer : IEqualityComparer<Point?>
    {
        public static readonly NullablePointComparer TwoDigits = new(2);

        private readonly int digits;

        public NullablePointComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Point? x, Point? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return PointComparer.Equals(x.Value, y.Value, this.digits);
        }

        public int GetHashCode(Point? obj)
        {
            return obj?.Round(this.digits).GetHashCode() ?? 0;
        }
    }
}
