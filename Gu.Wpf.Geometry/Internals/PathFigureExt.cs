namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    internal static class PathFigureExt
    {
        internal static double TotalLength(this PathFigure pathFigure)
        {
            PolyLineSegment polylineSegment = pathFigure.Segments[0] as PolyLineSegment;
            if (polylineSegment == null)
            {
                throw new NotSupportedException("Segment is not PolylineSegment in flattened PathFigure");
            }

            var points = polylineSegment.Points;
            var totalLength = 0.0;
            var previous = pathFigure.StartPoint;
            foreach (var pt in points)
            {
                totalLength += (pt - previous).Length;
                previous = pt;
            }

            return totalLength;
        }

        internal static IReadOnlyList<Line> AsLines(this PathFigure figure)
        {
            var polylineSegment = figure.Segments.Single() as PolyLineSegment;
            if (polylineSegment != null)
            {
                var points = polylineSegment.Points;
                var lines = new Line[points.Count];
                var sp = figure.StartPoint;
                for (int i = 0; i < points.Count; i++)
                {
                    var ep = points[i];
                    lines[i] = new Line(sp, ep);
                    sp = ep;
                }

                return lines;
            }

            throw new NotSupportedException("Segment is not PolylineSegment in flattened PathFigure");
        }
    }
}
