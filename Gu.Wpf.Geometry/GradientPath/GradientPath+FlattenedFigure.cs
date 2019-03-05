namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    public partial class GradientPath
    {
        internal class FlattenedFigure
        {
            public readonly IReadOnlyList<FlattenedSegment> Segments;

            public FlattenedFigure(PathFigure figure, double strokeThickness)
            {
                var lines = GetLines(figure);
                var offsetLines1 = CreateOffsetLines(lines, -strokeThickness / 2);
                var offsetLines2 = CreateOffsetLines(lines, strokeThickness / 2);
                var segments = new FlattenedSegment[lines.Count];
                for (var i = 0; i < segments.Length; i++)
                {
                    var o1 = offsetLines1[i];
                    var o2 = offsetLines2[i];
                    segments[i] = new FlattenedSegment(lines[i], CreatePath(o1, o2));
                }

                this.Segments = segments;
                this.TotalLength = this.Segments.Sum(x => x.Line.Length);
            }

            public double TotalLength { get; }

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

            private static IReadOnlyList<Line> GetLines(PathFigure figure)
            {
                var lines = new List<Line>();
                var sp = figure.StartPoint;
                foreach (var segment in figure.Segments)
                {
                    switch (segment)
                    {
                        case LineSegment lineSegment:
                            if (sp != lineSegment.Point)
                            {
                                lines.Add(new Line(sp, lineSegment.Point));
                                sp = lineSegment.Point;
                            }

                            break;
                        case PolyLineSegment polyLineSegment:
                            foreach (var point in polyLineSegment.Points)
                            {
                                if (sp != point)
                                {
                                    lines.Add(new Line(sp, point));
                                    sp = point;
                                }
                            }

                            break;

                        default:
                            throw new NotSupportedException("Segment is not PolyLineSegment in flattened PathFigure");
                    }
                }

                return lines;
            }
        }
    }
}
