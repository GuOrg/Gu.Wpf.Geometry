namespace Gu.Wpf.Geometry
{
    using System.Collections.Generic;
    using System.Windows.Media;

    partial class GradientPath
    {
        internal class FigureGeometry
        {
            public readonly IReadOnlyList<Line> Lines;
            public readonly PathGeometry[] PathGeometries;

            private readonly IReadOnlyList<Line> _offsetLines1;
            private readonly IReadOnlyList<Line> _offsetLines2;

            public FigureGeometry(PathFigure figure, double strokeThickness)
            {
                this.Lines = figure.AsLines();
                this._offsetLines1 = CreateOffsetLines(this.Lines, -strokeThickness / 2);
                this._offsetLines2 = CreateOffsetLines(this.Lines, strokeThickness / 2);
                var pathGeometries = new PathGeometry[this.Lines.Count];
                for (var i = 0; i < this.Lines.Count; i++)
                {
                    var o1 = this._offsetLines1[i];
                    var o2 = this._offsetLines2[i];
                    pathGeometries[i] = CreatePath(o1, o2);
                }
                this.PathGeometries = pathGeometries;
                this.TotalLength = figure.TotalLength();
                this.Brushes = new Brush[this.Lines.Count];
            }

            public double TotalLength { get; }

            public Brush[] Brushes { get; }

            internal static IReadOnlyList<Line> CreateOffsetLines(IReadOnlyList<Line> lines, double offset)
            {
                var result = new Line[lines.Count];
                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i].Offset(offset);
                    if (i > 0)
                    {
                        var previous = result[i - 1];
                        var extended = previous.TrimOrExtendEndWith(line);
                        if (extended == null)
                        {
                            continue;
                        }

                        previous = extended.Value;
                        result[i - 1] = previous;
                        line = new Line(previous.EndPoint, line.EndPoint);
                    }

                    result[i] = line;
                }
                return result;
            }

            private static PathGeometry CreatePath(Line l1, Line l2)
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
                return geometry;
            }
        }
    }
}
