namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct Line
    {
        public readonly Point StartPoint;
        public readonly Point EndPoint;

        public Line(Point startPoint, Point endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        internal Point MidPoint
        {
            get
            {
                var x = (this.StartPoint.X + this.EndPoint.X) / 2;
                var y = (this.StartPoint.Y + this.EndPoint.Y) / 2;
                return new Point(x, y);
            }
        }

        internal double Length => (this.EndPoint - this.StartPoint).Length;

        internal Vector Direction
        {
            get
            {
                var v = this.EndPoint - this.StartPoint;
                v.Normalize();
                return v;
            }
        }

        internal Vector PerpendicularDirection
        {
            get
            {
                var direction = this.Direction;
                return new Vector(direction.Y, -direction.X);
            }
        }

        private string DebuggerDisplay => $"{this.StartPoint.ToString("F1")} -> {this.EndPoint.ToString("F1")} length: {this.Length.ToString("F1")}";

        public override string ToString() => this.ToString(string.Empty);

        public string ToString(string format) => $"{this.StartPoint.ToString(format)}; {this.EndPoint.ToString(format)}";

        internal static Line Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }

            var sp = Point.Parse(strings[0]);
            var ep = Point.Parse(strings[1]);
            return new Line(sp, ep);
        }

        internal Line RotateAroundStartPoint(double angleInDegrees)
        {
            var v = this.EndPoint - this.StartPoint;
            v = v.Rotate(angleInDegrees);
            var ep = this.StartPoint + v;
            return new Line(this.StartPoint, ep);
        }

        internal Line Flip()
        {
            return new Line(this.EndPoint, this.StartPoint);
        }

        internal Line Offset(double distance)
        {
            var v = this.PerpendicularDirection;
            var sp = this.StartPoint.WithOffset(v, distance);
            var ep = this.EndPoint.WithOffset(v, distance);
            return new Line(sp, ep);
        }

        internal bool IsPointOnLine(Point p)
        {
            if (this.StartPoint.DistanceTo(p) < Constants.Tolerance)
            {
                return true;
            }

            var v = p - this.StartPoint;
            var angleBetween = Vector.AngleBetween(this.Direction, v);
            if (Math.Abs(angleBetween) > Constants.Tolerance)
            {
                return false;
            }

            return v.Length <= this.Length + Constants.Tolerance;
        }

        internal Point? TrimTo(Point p)
        {
            if (this.IsPointOnLine(p))
            {
                return p;
            }
            var v = this.StartPoint.VectorTo(p);
            if (Math.Abs(v.AngleTo(this.Direction) % 180) > Constants.Tolerance)
            {
                return null;
            }

            var dp = v.DotProdcut(this.Direction);
            return dp < 0
                       ? this.StartPoint
                       : this.EndPoint;
        }

        internal Point Project(Point p)
        {
            var toPoint = this.StartPoint.VectorTo(p);
            var dotProdcut = toPoint.DotProdcut(this.Direction);
            var projected = this.StartPoint + dotProdcut * this.Direction;
            return projected;
        }

        internal Line? TrimOrExtendEndWith(Line other)
        {
            if (this.EndPoint.DistanceTo(other.StartPoint) < Constants.Tolerance)
            {
                return this;
            }

            var ip = IntersectionPoint(this, other, false);
            if (ip == null)
            {
                return null;
            }

            return new Line(this.StartPoint, ip.Value);
        }

        internal Line? TrimOrExtendStartWith(Line other)
        {
            if (this.StartPoint.DistanceTo(other.EndPoint) < Constants.Tolerance)
            {
                return this;
            }

            var ip = IntersectionPoint(this, other, false);
            if (ip == null)
            {
                return null;
            }

            return new Line(ip.Value, this.EndPoint);
        }

        internal Point? IntersectWith(Line other, bool mustBeBetweenStartAndEnd)
        {
            return IntersectionPoint(this, other, mustBeBetweenStartAndEnd);
        }

        internal Point? ClosestIntersection(Rect rectangle)
        {
            var quadrant = rectangle.Contains(this.StartPoint)
                ? this.Direction.Quadrant()
                : this.Direction.Negated().Quadrant();

            switch (quadrant)
            {
                case Quadrant.NegativeXPositiveY:
                    return IntersectionPoint(rectangle.LeftLine(), this, true) ??
                           IntersectionPoint(rectangle.BottomLine(), this, true);
                case Quadrant.PositiveXPositiveY:
                    return IntersectionPoint(rectangle.RightLine(), this, true) ??
                           IntersectionPoint(rectangle.BottomLine(), this, true);
                case Quadrant.PositiveXNegativeY:
                    return IntersectionPoint(rectangle.RightLine(), this, true) ??
                           IntersectionPoint(rectangle.TopLine(), this, true);
                case Quadrant.NegativeXNegativeY:
                    return IntersectionPoint(rectangle.LeftLine(), this, true) ??
                           IntersectionPoint(rectangle.TopLine(), this, true);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal double DistanceTo(Point p)
        {
            return this.Project(p)
                       .DistanceTo(p);
        }

        internal double DistanceToPointOnLine(Point p)
        {
            var toPoint = this.StartPoint.VectorTo(p);
            var dotProdcut = toPoint.DotProdcut(this.Direction);
            var pointOnLine = this.StartPoint + dotProdcut * this.Direction;
            return pointOnLine.DistanceTo(p);
        }

        internal Line? PerpendicularLineTo(Point p)
        {
            if (this.IsPointOnLine(p))
            {
                return null;
            }

            var startPoint = this.Project(p);
            return new Line(startPoint, p);
        }

        // http://geomalgorithms.com/a05-_intersect-1.html#intersect2D_2Segments()
        private static Point? IntersectionPoint(Line l1, Line l2, bool mustBeBetweenStartAndEnd)
        {
            var u = l1.Direction;
            var v = l2.Direction;
            var w = l1.StartPoint - l2.StartPoint;
            var d = Perp(u, v);
            if (Math.Abs(d) < Constants.Tolerance)
            {
                // parallel lines
                return null;
            }
            var sI = Perp(v, w) / d;
            var p = l1.StartPoint + sI * u;
            if (mustBeBetweenStartAndEnd)
            {
                if (l1.IsPointOnLine(p) && l2.IsPointOnLine(p))
                {
                    return p;
                }

                return null;
            }

            return p;
        }

        private static double Perp(Vector u, Vector v)
        {
            return u.X * v.Y - u.Y * v.X;
        }
    }
}
