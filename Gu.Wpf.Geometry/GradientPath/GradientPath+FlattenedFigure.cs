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
                var segments = new List<FlattenedSegment>(lines.Count);
                for (int i = 0; i < lines.Count; i++)
                {
                    segments.Add(FlattenedSegment.Create(lines.ElementAtOrDefault(i - 1), lines[i], lines.ElementAtOrDefault(i + 1), strokeThickness));
                }

                this.Segments = segments;
                this.TotalLength = this.Segments.Sum(x => x.Line.Length);
            }

            public double TotalLength { get; }

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
