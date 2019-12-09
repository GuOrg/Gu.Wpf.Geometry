namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct Ellipse
    {
        internal readonly Point CenterPoint;
        internal readonly double RadiusX;
        internal readonly double RadiusY;

        internal Ellipse(Rect rect)
        {
            Debug.Assert(!rect.IsEmpty, "!rect.IsEmpty");
            this.RadiusX = (rect.Right - rect.X) * 0.5;
            this.RadiusY = (rect.Bottom - rect.Y) * 0.5;
            this.CenterPoint = new Point(rect.X + this.RadiusX, rect.Y + this.RadiusY);
        }

        internal Ellipse(Point centerPoint, double radiusX, double radiusY)
        {
            this.CenterPoint = centerPoint;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }

        internal bool IsZero => this.RadiusX <= 0 || this.RadiusY <= 0;

        private string DebuggerDisplay => $"{this.CenterPoint.ToString("F1")} rx: {this.RadiusX.ToString("F1", CultureInfo.InvariantCulture)} ry: {this.RadiusY.ToString("F1", CultureInfo.InvariantCulture)}";

        internal static Ellipse CreateFromSize(Size renderSize)
        {
            var width = renderSize.Width;
            var height = renderSize.Height;
            var rx = width / 2;
            var ry = height / 2;
            return new Ellipse(new Point(rx, ry), rx, ry);
        }

        internal static Ellipse Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 3)
            {
                throw new FormatException("Could not parse an Ellipse from the string.");
            }

            var cp = Point.Parse(strings[0]);
            var rx = double.Parse(strings[1], CultureInfo.InvariantCulture);
            var ry = double.Parse(strings[2], CultureInfo.InvariantCulture);
            return new Ellipse(cp, rx, ry);
        }

        // Not sure if radius makes any sense here, not very important since internal
        // http://math.stackexchange.com/a/687384/47614
        internal double RadiusInDirection(Vector directionFromCenter)
        {
            var angle = Math.Atan2(directionFromCenter.Y, directionFromCenter.X);
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var a = this.RadiusX;
            var b = this.RadiusY;
            var r2 = 1 / (cos * cos / (a * a) + sin * sin / (b * b));
            return Math.Sqrt(r2);
        }

        internal Point PointOnCircumference(Vector directionFromCenter)
        {
            var r = this.RadiusInDirection(directionFromCenter);
            return this.CenterPoint + r * directionFromCenter.Normalized();
        }

        internal bool Contains(Point p)
        {
            var v = this.CenterPoint.VectorTo(p);
            var r = this.RadiusInDirection(v);
            return this.CenterPoint.DistanceTo(p) <= r;
        }
    }
}
