namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public partial class GradientPath
    {
        internal class FlattenedSegment
        {
            internal readonly Line Line;
            internal readonly Geometry Geometry;
            internal Brush? Brush;

            internal FlattenedSegment(Line line, Geometry geometry)
            {
                this.Line = line;
                this.Geometry = geometry;
            }

            internal static FlattenedSegment Create(Line? previousCenter, Line center, Line? nextCenter, double strokeThickness)
            {
                return CreateOffset(center, Offset(strokeThickness / 2), Offset(-strokeThickness / 2));

                Line Offset(double offset)
                {
                    var ol = center.Offset(offset);
                    if (previousCenter is { } pl &&
                        pl.Length > 0 &&
                        ol.TrimOrExtendStartWith(pl.Offset(offset)) is { } temp1)
                    {
                        ol = temp1;
                    }

                    if (nextCenter is { } nl &&
                        nl.Length > 0 &&
                        ol.TrimOrExtendEndWith(nl.Offset(offset)) is { } temp2)
                    {
                        ol = temp2;
                    }

                    return ol;
                }
            }

            internal static FlattenedSegment CreateStartLineCap(Line line, PenLineCap startLineCap, double strokeThickness)
            {
                switch (startLineCap)
                {
                    case PenLineCap.Square:
                        var cl = new Line(line.StartPoint - (strokeThickness / 2 * line.Direction), line.StartPoint);
                        return CreateOffset(cl, cl.Offset(strokeThickness / 2), cl.Offset(-strokeThickness / 2));
                    case PenLineCap.Round:
                        return CreateRound(line, strokeThickness, isStart: true);
                    case PenLineCap.Triangle:
                        return CreateTriangle(line, strokeThickness, isStart: true);
                    case PenLineCap.Flat:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(startLineCap), startLineCap, "Unknown line cap.");
                }
            }

            internal static FlattenedSegment CreateEndLineCap(Line line, PenLineCap endLineCap, double strokeThickness)
            {
                switch (endLineCap)
                {
                    case PenLineCap.Square:
                        var cl = new Line(line.EndPoint, line.EndPoint + (strokeThickness / 2 * line.Direction));
                        return CreateOffset(cl, cl.Offset(strokeThickness / 2), cl.Offset(-strokeThickness / 2));
                    case PenLineCap.Round:
                        return CreateRound(line, strokeThickness, isStart: false);
                    case PenLineCap.Triangle:
                        return CreateTriangle(line, strokeThickness, isStart: false);
                    case PenLineCap.Flat:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(endLineCap), endLineCap, "Unknown line cap.");
                }
            }

            private static FlattenedSegment CreateOffset(Line center, Line l1, Line l2)
            {
                var geometry = new PathGeometry
                {
                    Figures =
                    {
                        new PathFigure
                        {
                            StartPoint = l1.StartPoint,
                            IsClosed = true,
                            IsFilled = true,
                            Segments =
                            {
                                new PolyLineSegment
                                {
                                    IsSmoothJoin = false,
                                    Points =
                                    {
                                        l1.EndPoint,
                                        l2.EndPoint,
                                        l2.StartPoint,
                                    },
                                },
                            },
                        },
                    },
                };
                return new FlattenedSegment(center, geometry);
            }

            private static FlattenedSegment CreateRound(Line center, double strokeThickness, bool isStart)
            {
                var radius = strokeThickness / 2;
                var line = isStart
                    ? new Line(center.StartPoint - (radius * center.Direction), center.StartPoint)
                    : new Line(center.EndPoint, center.EndPoint + (radius * center.Direction));
                var geometry = new PathGeometry
                {
                    Figures =
                    {
                        new PathFigure
                        {
                            StartPoint = isStart
                                ? line.EndPoint + (radius * line.Direction.Rotate(-90))
                                : line.StartPoint + (radius * line.Direction.Rotate(90)),
                            IsClosed = true,
                            IsFilled = true,
                            Segments =
                            {
                                new ArcSegment
                                {
                                   IsLargeArc = false,
                                   RotationAngle = 180,
                                   Point = isStart
                                        ? line.EndPoint + (radius * line.Direction.Rotate(90))
                                        : line.StartPoint + (radius * line.Direction.Rotate(-90)),
                                   Size = new Size(radius, radius),
                                   IsSmoothJoin = false,
                                },
                            },
                        },
                    },
                };

                return new FlattenedSegment(line, geometry);
            }

            private static FlattenedSegment CreateTriangle(Line center, double strokeThickness, bool isStart)
            {
                var radius = strokeThickness / 2;
                var line = isStart
                    ? new Line(center.StartPoint - (radius * center.Direction), center.StartPoint)
                    : new Line(center.EndPoint, center.EndPoint + (radius * center.Direction));

                var geometry = new PathGeometry
                {
                    Figures =
                    {
                        new PathFigure
                        {
                            StartPoint = isStart
                                ? line.EndPoint + (radius * line.Direction.Rotate(-90))
                                : line.StartPoint + (radius * line.Direction.Rotate(90)),
                            IsClosed = true,
                            IsFilled = true,
                            Segments =
                            {
                                new PolyLineSegment
                                {
                                    IsSmoothJoin = false,
                                    Points =
                                    {
                                        isStart ? line.StartPoint : line.EndPoint,
                                        isStart ? line.EndPoint + (radius * line.Direction.Rotate(90)) : line.StartPoint + (radius * line.Direction.Rotate(-90)),
                                    },
                                },
                            },
                        },
                    },
                };

                return new FlattenedSegment(line, geometry);
            }
        }
    }
}
