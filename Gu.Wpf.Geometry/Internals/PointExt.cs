namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    internal static class PointExt
    {
        internal static Point WithOffset(this Point self, Vector direction, double distance)
        {
            return self + distance * direction;
        }

        internal static Point WithOffset(this Point self, double x, double y)
        {
            return new Point(self.X + x, self.Y + y);
        }

        internal static double DistanceTo(this Point self, Point other)
        {
            return (self - other).Length;
        }

        internal static double DistanceTo(this Point self, Line other)
        {
            return other.DistanceToPointOnLine(self);
        }

        internal static Point Round(this Point self, int digits = 0)
        {
            return new Point(Math.Round(self.X, digits), Math.Round(self.Y, digits));
        }

        internal static Vector VectorTo(this Point self, Point other)
        {
            return other - self;
        }

        internal static Line LineTo(this Point self, Point other)
        {
            return new Line(self, other);
        }

        internal static Vector VectorToTangent(this Point self, Circle circle, Sign rotationDirection)
        {
            var toCenter = self.VectorTo(circle.Center);
            var angle = rotationDirection == Sign.Positive ? 90 : -90;
            var perp = circle.Radius * toCenter.Rotate(angle).Normalized();
            var toTangent = self.VectorTo(circle.Center + perp);
            return toTangent;
        }

        internal static Point? TrimToLine(this Point self, Line l)
        {
            return l.TrimTo(self);
        }

        internal static Point ClosestPointOn(this Point self, Line l)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return l.TrimTo(l.Project(self)).Value;
        }

        internal static Point Closest(this Point self, Point p1, Point p2)
        {
            return self.DistanceTo(p1) < self.DistanceTo(p2) ? p1 : p2;
        }

        internal static Point Closest(this Point self, Point p1, Point p2, Point p3, Point p4)
        {
            return self.Closest(self.Closest(p1, p2), self.Closest(p3, p4));
        }

        internal static Line Closest(this Point self, Line l1, Line l2)
        {
            return l1.DistanceTo(self) < l2.DistanceTo(self) ? l1 : l2;
        }

        internal static Line Closest(this Point self, Line l1, Line l2, Line l3, Line l4)
        {
            return self.Closest(self.Closest(l1, l2), self.Closest(l3, l4));
        }

        internal static Point MidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        internal static Point ToScreen(this Point self, UIElement element)
        {
            if (PresentationSource.FromVisual(element) == null)
            {
                return self;
            }

            return element.PointToScreen(self);
        }

        internal static string ToString(this Point? self, string format = "F1")
        {
            return self == null ? "null" : self.Value.ToString(format);
        }

        internal static string ToString(this Point self, string format = "F1")
        {
            return $"{self.X.ToString(format, CultureInfo.InvariantCulture)},{self.Y.ToString(format, CultureInfo.InvariantCulture)}";
        }
    }
}
