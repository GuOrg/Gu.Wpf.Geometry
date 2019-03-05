namespace Gu.Wpf.Geometry
{
    using System.Windows.Media;

    public partial class GradientPath
    {
        internal class FlattenedSegment
        {
            public readonly Line Line;
            public readonly Geometry Geometry;
            public Brush Brush;

            public FlattenedSegment(Line line, Geometry geometry)
            {
                this.Line = line;
                this.Geometry = geometry;
            }

            public static FlattenedSegment Create(Line? previousCenter, Line center, Line? nextCenter, double strokeThickness)
            {
                return CreateOffset(center, Offset(strokeThickness / 2), Offset(-strokeThickness / 2));

                Line Offset(double offset)
                {
                    var ol = center.Offset(offset);
                    if (previousCenter is Line pl &&
                        pl.Length > 0 &&
                        ol.TrimOrExtendStartWith(pl.Offset(offset)) is Line temp1)
                    {
                        ol = temp1;
                    }

                    if (nextCenter is Line nl &&
                        nl.Length > 0 &&
                        ol.TrimOrExtendEndWith(nl.Offset(offset)) is Line temp2)
                    {
                        ol = temp2;
                    }

                    return ol;
                }
            }

            public static FlattenedSegment CreateOffset(Line center, Line l1, Line l2)
            {
                var geometry = new PathGeometry();
                var figure = new PathFigure
                {
                    StartPoint = l1.StartPoint,
                    IsClosed = true,
                    IsFilled = true,
                };
                var polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points.Add(l1.EndPoint);
                polyLineSegment.Points.Add(l2.EndPoint);
                polyLineSegment.Points.Add(l2.StartPoint);
                figure.Segments.Add(polyLineSegment);
                geometry.Figures.Add(figure);
                return new FlattenedSegment(center, geometry);
            }
        }
    }
}
