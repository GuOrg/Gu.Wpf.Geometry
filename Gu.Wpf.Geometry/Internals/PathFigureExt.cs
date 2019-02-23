namespace Gu.Wpf.Geometry
{
    using System;
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
    }
}
