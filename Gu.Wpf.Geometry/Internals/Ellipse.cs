namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct Ellipse
    {
        internal readonly Point Center;
        internal readonly double RadiusX;
        internal readonly double RadiusY;

        internal Ellipse(Rect rect)
        {
            Debug.Assert(!rect.IsEmpty, "!rect.IsEmpty");
            this.RadiusX = (rect.Right - rect.X) * 0.5;
            this.RadiusY = (rect.Bottom - rect.Y) * 0.5;
            this.Center = new Point(rect.X + this.RadiusX, rect.Y + this.RadiusY);
        }

        internal Ellipse(Point center, double radiusX, double radiusY)
        {
            this.Center = center;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }

        internal bool IsZero => this.RadiusX <= 0 || this.RadiusY <= 0;

        private string DebuggerDisplay => $"{this.Center.ToString("F1")} rx: {this.RadiusX.ToString("F1")} ry: {this.RadiusY.ToString("F1")}";

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
                throw new ArgumentException();
            }

            var cp = Point.Parse(strings[0]);
            var rx = double.Parse(strings[1]);
            var ry = double.Parse(strings[2]);
            return new Ellipse(cp, rx,ry);
        }

        // Not sure if radius makes any sense here, not very important since internal
        internal double RadiusInDirection(Vector directionFromCenter)
        {
            var a = new Vector(1, 0).AngleTo(directionFromCenter) * Constants.DegToRad;
            var rx = this.RadiusX * Math.Cos(a);
            var ry = this.RadiusY * Math.Sin(a);
            return Math.Sqrt(rx * rx + ry * ry);
        }

        internal Point PointOnCircumference(Vector directionFromCenter)
        {
            var a = Math.Atan2(directionFromCenter.Y, directionFromCenter.X);
            var x = this.Center.X + this.RadiusX * Math.Cos(a);
            var y = this.Center.Y + this.RadiusY * Math.Sin(a);
            return new Point(x, y);
        }
    }
}