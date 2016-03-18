namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    internal static class VectorExt
    {
        private const double DegToRad = Math.PI / 180;

        internal static Quadrant Quadrant(this Vector v)
        {
            if (v.X >= 0)
            {
                return v.Y >= 0
                           ? Geometry.Quadrant.TopRight
                           : Geometry.Quadrant.BottomRight;
            }

            return v.Y >= 0
                       ? Geometry.Quadrant.TopLeft
                       : Geometry.Quadrant.BottomLeft;
        }

        internal static Axis? Axis(this Vector v, double tolerance = Constants.Tolerance)
        {
            if (Math.Abs(v.X) < tolerance)
            {
                return v.Y > 0
                           ? Geometry.Axis.PositiveY
                           : Geometry.Axis.NegativeY;
            }

            if (Math.Abs(v.Y) < tolerance)
            {
                return v.X > 0
                           ? Geometry.Axis.PositiveX
                           : Geometry.Axis.NegativeX;
            }

            return null;
        }

        internal static double AngleTo(this Vector v, Vector other)
        {
            return Vector.AngleBetween(v, other);
        }

        internal static Vector Rotate(this Vector v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        internal static double DotProdcut(this Vector v, Vector other)
        {
            return Vector.Multiply(v, other);
        }

        internal static Vector ProjectOn(this Vector v, Vector other)
        {
            var dp = v.DotProdcut(other);
            return dp * other;
        }

        internal static Vector RotateRadians(this Vector v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        internal static Vector Normalized(this Vector v)
        {
            var uv = new Vector(v.X, v.Y);
            uv.Normalize();
            return uv;
        }

        internal static Vector Negated(this Vector v)
        {
            var negated = new Vector(v.X, v.Y);
            negated.Negate();
            return negated;
        }

        internal static Vector Round(this Vector v, int digits = 0)
        {
            return new Vector(Math.Round(v.X, digits), Math.Round(v.Y, digits));
        }

        internal static string ToString(this Vector? self, string format = "F1")
        {
            return self == null ? "null" : self.Value.ToString(format);
        }

        internal static string ToString(this Vector self, string format = "F1")
        {
            return $"{self.X.ToString(format, CultureInfo.InvariantCulture)},{self.Y.ToString(format, CultureInfo.InvariantCulture)}";
        }
    }
}
