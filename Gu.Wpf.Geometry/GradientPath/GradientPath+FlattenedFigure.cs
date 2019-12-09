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
            internal readonly IReadOnlyList<FlattenedSegment> Segments;
            internal readonly double TotalLength;
#pragma warning disable CA1825
            private static readonly FlattenedSegment[] EmptySegments = new FlattenedSegment[0];
#pragma warning restore CA1825

            internal FlattenedFigure(PathFigure figure, PenLineCap startLineCap, PenLineCap endLineCap, double strokeThickness)
            {
                var lines = GetLines(figure);
                if (!lines.Any())
                {
                    this.Segments = EmptySegments;
                    return;
                }

                var segments = new List<FlattenedSegment>(lines.Count);
                switch (startLineCap)
                {
                    case PenLineCap.Flat:
                        break;
                    case PenLineCap.Square:
                    case PenLineCap.Round:
                    case PenLineCap.Triangle:
                        segments.Add(FlattenedSegment.CreateStartLineCap(lines[0], startLineCap, strokeThickness));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(startLineCap), startLineCap, "Unknown line cap.");
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    segments.Add(FlattenedSegment.Create(lines.ElementAtOrDefault(i - 1), lines[i], lines.ElementAtOrDefault(i + 1), strokeThickness));
                }

                switch (endLineCap)
                {
                    case PenLineCap.Flat:
                        break;
                    case PenLineCap.Square:
                    case PenLineCap.Round:
                    case PenLineCap.Triangle:
                        segments.Add(FlattenedSegment.CreateEndLineCap(lines[lines.Count - 1], endLineCap, strokeThickness));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(endLineCap), endLineCap, "Unknown line cap.");
                }

                this.Segments = segments;
                this.TotalLength = this.Segments.Sum(x => x.Line.Length);
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
