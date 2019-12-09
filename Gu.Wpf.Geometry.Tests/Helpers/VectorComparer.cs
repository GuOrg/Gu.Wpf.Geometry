namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public sealed class VectorComparer : IEqualityComparer<Vector>
    {
        public static readonly VectorComparer TwoDigits = new VectorComparer(2);

        private readonly int digits;

        private VectorComparer(int digits)
        {
            this.digits = digits;
        }

        public static bool Equals(Vector x, Vector y, int decimalDigits)
        {
            return x.Round(decimalDigits) == y.Round(decimalDigits);
        }

        public bool Equals(Vector x, Vector y)
        {
            return Equals(x, y, this.digits);
        }

        public int GetHashCode(Vector obj)
        {
            return obj.Round(this.digits).GetHashCode();
        }
    }
}
