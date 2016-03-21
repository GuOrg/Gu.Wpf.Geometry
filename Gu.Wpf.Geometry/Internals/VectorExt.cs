namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    internal static class VectorExt
    {
        internal static Quadrant Quadrant(this Vector v)
        {
            if (v.X >= 0)
            {
                return v.Y >= 0
                           ? Geometry.Quadrant.PositiveXPositiveY
                           : Geometry.Quadrant.PositiveXNegativeY;
            }

            return v.Y >= 0
                       ? Geometry.Quadrant.NegativeXPositiveY
                       : Geometry.Quadrant.NegativeXNegativeY;
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
            return v.RotateRadians(degrees * Constants.DegToRad);
        }

        internal static double AngleToPositiveX(this Vector v)
        {
            var angle = Math.Atan2(v.Y, v.X) * Constants.RadToDeg;
            return angle;
        }

        internal static Vector? SnapToOrtho(this Vector v)
        {
            var angle = v.AngleToPositiveX();
            if (-135 < angle && angle < -45)
            {
                return new Vector(0, -v.Length);
            }

            if (-45 < angle && angle < 45)
            {
                return new Vector(v.Length, 0);
            }

            if (45 < angle && angle < 135)
            {
                return new Vector(0, v.Length);
            }

            if ((-180 <= angle && angle < -135) || (135 < angle && angle <= 180))
            {
                return new Vector(-v.Length, 0);
            }
            return null;
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
