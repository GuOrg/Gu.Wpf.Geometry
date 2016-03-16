namespace Gu.Wpf.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    partial class GradientPath
    {
        private class GradientGeometry
        {
            private System.Windows.Media.Geometry _data;
            private double _tolerance;
            private readonly double _strokeThickness;
            public readonly IReadOnlyList<FigureGeometry> FigureGeometries;

            public GradientGeometry(System.Windows.Media.Geometry data, double tolerance, double strokeThickness)
            {
                _data = data;
                _tolerance = tolerance;
                _strokeThickness = strokeThickness;
                // Flatten the PathGeometry
                var flattened = data.GetFlattenedPathGeometry(tolerance, ToleranceType.Absolute);
                FigureGeometries = flattened.Figures.Select(x => new FigureGeometry(x, strokeThickness)).ToArray();
            }
        }
    }
}
