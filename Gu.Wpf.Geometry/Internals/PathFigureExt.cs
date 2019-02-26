namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows.Media;

    internal static class PathFigureExt
    {
        internal static double TotalLength(this PathFigure pathFigure)
        {
            var totalLength = 0.0;
            var previous = pathFigure.StartPoint;

            foreach (var segment in pathFigure.Segments)
            {
                switch (segment)
                {
                    case LineSegment lineSegment:
                        totalLength += (lineSegment.Point - previous).Length;
                        previous = lineSegment.Point;
                        break;

                    case PolyLineSegment polyLineSegment:
                        var points = polyLineSegment.Points;

                        foreach (var point in polyLineSegment.Points)
                        {
                            totalLength += (point - previous).Length;
                            previous = point;
                        }

                        break;

                    default:
                        throw new NotSupportedException("Segment is not PolyLineSegment or LineSegment in flattened PathFigure");
                }
            }

            return totalLength;
        }
    }
}
