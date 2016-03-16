namespace Gu.Wpf.Geometry
{
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("StartPoint: {StartPoint.ToDebugString()} EndPoint: {EndPoint.ToDebugString()}")]
    internal struct Line
    {
        public readonly Point StartPoint;
        public readonly Point EndPoint;

        public Line(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Point MidPoint
        {
            get
            {
                var x = (StartPoint.X + EndPoint.X) / 2;
                var y = (StartPoint.Y + EndPoint.Y) / 2;
                return new Point(x, y);
            }
        }

        public double Length => (EndPoint - StartPoint).Length;

        public Vector Direction
        {
            get
            {
                var v = EndPoint - StartPoint;
                v.Normalize();
                return v;
            }
        }

        public Vector PerpendicularDirection
        {
            get
            {
                var direction = Direction;
                return new Vector(direction.Y, -direction.X);
            }
        }

        public string ToString(string format)
        {
            return $"{StartPoint.ToDebugString(format)}; {EndPoint.ToDebugString(format)}";
        }

        public Line Offset(double distance)
        {
            var v = PerpendicularDirection;
            var sp = StartPoint.Offset(v, distance);
            var ep = EndPoint.Offset(v, distance);
            return new Line(sp, ep);
        }

        internal Line TrimOrExtendEndWith(Line other)
        {
            if (EndPoint.DistanceTo(other.StartPoint) < 1e-3)
            {
                return this;
            }
            var ip = IntersectionPoint(this, other);
            return new Line(StartPoint, ip);
        }

        internal Line TrimOrExtendStartWith(Line other)
        {
            if (StartPoint.DistanceTo(other.EndPoint) < 1e-3)
            {
                return this;
            }
            var ip = IntersectionPoint(this, other);
            return new Line(ip, EndPoint);
        }

        /// <summary>
        /// http://geomalgorithms.com/a05-_intersect-1.html#intersect2D_2Segments()
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        internal static Point IntersectionPoint(Line l1, Line l2)
        {
            var u = l1.Direction;
            var v = l2.Direction;
            var w = l1.StartPoint - l2.StartPoint;
            var d = perp(u, v);
            var sI = perp(v, w) / d;
            var p = l1.StartPoint + sI * u;
            return p;
        }

        private static double perp(Vector u, Vector v)
        {
            return ((u).X * (v).Y - (u).Y * (v).X);
        }
    }
}
