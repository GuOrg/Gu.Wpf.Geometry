namespace Gu.Wpf.Geometry;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct Ray
{
    internal readonly Point Point;
    internal readonly Vector Direction;

    internal Ray(Point point, Vector direction)
    {
        this.Point = point;
        this.Direction = direction.Normalized();
    }

    private string DebuggerDisplay => $"{this.Point.ToString("F1")}, {this.Direction.ToString("F1")} angle: {this.Direction.AngleToPositiveX().ToString("F1", CultureInfo.InvariantCulture)}";

    internal static Ray Parse(string text)
    {
        var strings = text.Split(';');
        if (strings.Length != 2)
        {
            throw new FormatException("Could not parse a Ray from the string.");
        }

        var p = Point.Parse(strings[0]);
        var v = Vector.Parse(strings[1]).Normalized();
        return new Ray(p, v);
    }

    internal Ray Rotate(double angleInDegrees)
    {
        return new Ray(this.Point, this.Direction.Rotate(angleInDegrees));
    }

    internal bool IsPointOn(Point p)
    {
        if (this.Point.DistanceTo(p) < Constants.Tolerance)
        {
            return true;
        }

        var angle = this.Point.VectorTo(p).AngleTo(this.Direction);
        return Math.Abs(angle) < Constants.Tolerance;
    }

    internal Ray Flip()
    {
        return new Ray(this.Point, this.Direction.Negated());
    }

    internal Point Project(Point p)
    {
        var toPoint = this.Point.VectorTo(p);
        var dotProdcut = toPoint.DotProdcut(this.Direction);
        var projected = this.Point + (dotProdcut * this.Direction);
        return projected;
    }

    internal Line? PerpendicularLineTo(Point p)
    {
        if (this.IsPointOn(p))
        {
            return null;
        }

        var startPoint = this.Project(p);
        return new Line(startPoint, p);
    }

    internal Point? FirstIntersectionWith(Rect rectangle)
    {
        var quadrant = rectangle.Contains(this.Point)
            ? this.Direction.Quadrant()
            : this.Direction.Negated().Quadrant();

        return quadrant switch
        {
            Quadrant.NegativeXPositiveY
            => IntersectionPoint(this, rectangle.LeftLine(), mustBeBetweenStartAndEnd: true) ??
               IntersectionPoint(this, rectangle.BottomLine(), mustBeBetweenStartAndEnd: true),
            Quadrant.PositiveXPositiveY
            => IntersectionPoint(this, rectangle.RightLine(), mustBeBetweenStartAndEnd: true) ??
               IntersectionPoint(this, rectangle.BottomLine(), mustBeBetweenStartAndEnd: true),
            Quadrant.PositiveXNegativeY
            => IntersectionPoint(this, rectangle.RightLine(), mustBeBetweenStartAndEnd: true) ??
               IntersectionPoint(this, rectangle.TopLine(), mustBeBetweenStartAndEnd: true),
            Quadrant.NegativeXNegativeY
            => IntersectionPoint(this, rectangle.LeftLine(), mustBeBetweenStartAndEnd: true) ??
               IntersectionPoint(this, rectangle.TopLine(), mustBeBetweenStartAndEnd: true),
            _ => throw new InvalidEnumArgumentException("Unhandled Quadrant."),
        };
    }

    internal Point? FirstIntersectionWith(Circle circle)
    {
        var perp = this.PerpendicularLineTo(circle.Center);
        if (perp is null)
        {
            return this.Point.DistanceTo(circle.Center) < circle.Radius
                ? circle.Center + (circle.Radius * this.Direction)
                : circle.Center - (circle.Radius * this.Direction);
        }

        var pl = perp.Value.Length;
        if (pl > circle.Radius)
        {
            return null;
        }

        var tangentLength = Math.Sqrt((circle.Radius * circle.Radius) - (pl * pl));
        var result = perp.Value.StartPoint - (tangentLength * this.Direction);
        return this.IsPointOn(result)
                   ? (Point?)result
                   : null;
    }

    // http://www.mare.ee/indrek/misc/2d.pdf
    internal Point? FirstIntersectionWith(Ellipse ellipse)
    {
        if (Math.Abs(ellipse.CenterPoint.X) < Constants.Tolerance && Math.Abs(ellipse.CenterPoint.Y) < Constants.Tolerance)
        {
            return FirstIntersectionWithEllipseCenteredAtOrigin(this.Point, this.Direction, ellipse.RadiusX, ellipse.RadiusY);
        }

        var offset = new Point(0, 0).VectorTo(ellipse.CenterPoint);
        var ip = FirstIntersectionWithEllipseCenteredAtOrigin(this.Point - offset, this.Direction, ellipse.RadiusX, ellipse.RadiusY);
        return ip + offset;
    }

    private static Point? FirstIntersectionWithEllipseCenteredAtOrigin(Point startPoint, Vector direction, double a, double b)
    {
        var nx = direction.X;
        var nx2 = nx * nx;
        var ny = direction.Y;
        var ny2 = ny * ny;
        var x0 = startPoint.X;
        var x02 = x0 * x0;
        var y0 = startPoint.Y;
        var y02 = y0 * y0;
        var a2 = a * a;
        var b2 = b * b;
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
        var A = (nx2 * b2) + (ny2 * a2);
        if (Math.Abs(A) < Constants.Tolerance)
        {
            return null;
        }

        var B = (2 * x0 * nx * b2) + (2 * y0 * ny * a2);
        var C = (x02 * b2) + (y02 * a2) - (a2 * b2);
        var d = (B * B) - (4 * A * C);
        if (d < 0)
        {
            return null;
        }

        var sqrt = Math.Sqrt(d);
        var s = (-B - sqrt) / (2 * A);
        if (s < 0)
        {
            s = (-B + sqrt) / (2 * A);
            return s > 0
                ? new Point(x0, y0) + (s * direction)
                : (Point?)null;
        }

        return new Point(x0, y0) + (s * direction);
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
    }

    // http://geomalgorithms.com/a05-_intersect-1.html#intersect2D_2Segments()
    private static Point? IntersectionPoint(Ray ray, Line l2, bool mustBeBetweenStartAndEnd)
    {
        var u = ray.Direction;
        var v = l2.Direction;
        var w = ray.Point - l2.StartPoint;
        var d = Perp(u, v);
        if (Math.Abs(d) < Constants.Tolerance)
        {
            // parallel lines
            return null;
        }

        var sI = Perp(v, w) / d;
        var p = ray.Point + (sI * u);
        if (mustBeBetweenStartAndEnd)
        {
            if (ray.IsPointOn(p) && l2.IsPointOnLine(p))
            {
                return p;
            }

            return null;
        }

        return p;
    }

    private static double Perp(Vector u, Vector v)
    {
        return (u.X * v.Y) - (u.Y * v.X);
    }
}
