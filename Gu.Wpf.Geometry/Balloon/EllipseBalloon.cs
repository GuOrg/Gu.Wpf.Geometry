namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Media;

    public class EllipseBalloon : BalloonBase
    {
        protected override Geometry GetOrCreateBoxGeometry(Size renderSize)
        {
            var width = renderSize.Width - this.StrokeThickness;
            var height = renderSize.Height - this.StrokeThickness;
            if (width <= 0 || height <= 0)
            {
                if (this.StrokeThickness <= 0)
                {
                    return Geometry.Empty;
                }

                return new EllipseGeometry(new Point(this.StrokeThickness / 2, this.StrokeThickness / 2), this.StrokeThickness, this.StrokeThickness);
            }

            var rect = new Rect(new Point(0, 0), new Size(width, height));
            var geometry = new EllipseGeometry(rect);
            geometry.Freeze();
            return geometry;
        }

        protected override Geometry GetOrCreateConnectorGeometry(Size renderSize)
        {
            if (this.ConnectorOffset == default(Vector) || renderSize.IsEmpty)
            {
                return Geometry.Empty;
            }

            var rectangle = new Rect(new Point(0, 0), renderSize);
            rectangle.Inflate(-this.StrokeThickness, -this.StrokeThickness);
            if (rectangle.IsEmpty)
            {
                return Geometry.Empty;
            }

            return Geometry.Empty;
            //var width = renderSize.Width - this.StrokeThickness;
            //var height = renderSize.Height - this.StrokeThickness;
            //var mp = new Point(width / 2, height / 2);
            //var direction = this.ConnectorOffset.Normalized();
            //var length = width * width + height * height;
            //var line = new Line(mp, mp + length * direction);

            //var ip = line.ClosestIntersection(rectangle);
            //if (ip == null)
            //{
            //    Debug.Assert(false, $"Line {line} does not intersect rectangle {rectangle}");
            //    // ReSharper disable once HeuristicUnreachableCode
            //    return Geometry.Empty;
            //}

            //var cr = this.AdjustedCornerRadius();
            //var sp = ip.Value + this.ConnectorOffset;
            //line = new Line(sp, mp + length * direction.Negated());
            //var p1 = ConnectorPoint.Find(line, this.ConnectorAngle / 2, rectangle, cr);
            //var p2 = ConnectorPoint.Find(line, -this.ConnectorAngle / 2, rectangle, cr);

            //var geometry = new StreamGeometry();
            //using (var context = geometry.Open())
            //{
            //    context.BeginFigure(sp, true, true);
            //    context.LineTo(p1, true, true);
            //    context.LineTo(p2, true, true);
            //}

            //geometry.Freeze();
            //return geometry;
        }
    }
}