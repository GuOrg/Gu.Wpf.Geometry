namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class NullableVectorComparer : IEqualityComparer<Vector?>
    {
        private readonly int digits;
        public static readonly NullableVectorComparer Default = new NullableVectorComparer(2);

        public NullableVectorComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Vector? x, Vector? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return VectorComparer.Default.Equals(x.Value, y.Value, this.digits);
        }

        public int GetHashCode(Vector? obj)
        {
            return obj?.Round(this.digits).GetHashCode() ?? 0;
        }
    }
}