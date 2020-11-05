namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct Circle
    {
        internal readonly Point Center;
        internal readonly double Radius;

        internal Circle(Point center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        private string DebuggerDisplay => $"{this.Center.ToString("F1")} radius: {this.Radius.ToString("F1", CultureInfo.InvariantCulture)}";

        internal static Circle Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 2)
            {
                throw new FormatException("Could not parse a Circle from the string.");
            }

            var cp = Point.Parse(strings[0]);
            var r = double.Parse(strings[1], CultureInfo.InvariantCulture);
            return new Circle(cp, r);
        }

        internal Point? ClosestIntersection(Line line)
        {
            var perp = line.PerpendicularLineTo(this.Center);
            if (perp is null)
            {
                return this.Center - (this.Radius * line.Direction);
            }

            var pl = perp.Value.Length;
            if (pl > this.Radius)
            {
                return null;
            }

            var tangentLength = Math.Sqrt((this.Radius * this.Radius) - (pl * pl));
            return perp.Value.StartPoint - (tangentLength * line.Direction);
        }

        internal Point PointOnCircumference(Vector directionFromCenter)
        {
            var a = Math.Atan2(directionFromCenter.Y, directionFromCenter.X);
            var x = this.Center.X + (this.Radius * Math.Cos(a));
            var y = this.Center.Y + (this.Radius * Math.Sin(a));
            return new Point(x, y);
        }
    }
}
