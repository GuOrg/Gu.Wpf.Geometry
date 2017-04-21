﻿namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class VectorComparer : IEqualityComparer<Vector>
    {
        public static readonly VectorComparer TwoDigits = new VectorComparer(2);

        private readonly int digits;

        private VectorComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Vector x, Vector y)
        {
            return this.Equals(x, y, this.digits);
        }

        public bool Equals(Vector x, Vector y, int decimalDigits)
        {
            return x.Round(decimalDigits) == y.Round(decimalDigits);
        }

        public int GetHashCode(Vector obj)
        {
            return obj.Round(this.digits).GetHashCode();
        }
    }
}