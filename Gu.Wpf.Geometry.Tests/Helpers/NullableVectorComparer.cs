namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public sealed class NullableVectorComparer : IEqualityComparer<Vector?>
    {
        public static readonly NullableVectorComparer TwoDigits = new NullableVectorComparer(2);

        private readonly int digits;

        public NullableVectorComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Vector? x, Vector? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return VectorComparer.Equals(x.Value, y.Value, this.digits);
        }

        public int GetHashCode(Vector? obj)
        {
            return obj?.Round(this.digits).GetHashCode() ?? 0;
        }
    }
}
