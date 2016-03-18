namespace Gu.Wpf.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    partial class GradientPath
    {
        private class GradientGeometry
        {
            private Geometry _data;
            private double _tolerance;
            private readonly double _strokeThickness;
            public readonly IReadOnlyList<FigureGeometry> FigureGeometries;

            public GradientGeometry(Geometry data, double tolerance, double strokeThickness)
            {
                this._data = data;
                this._tolerance = tolerance;
                this._strokeThickness = strokeThickness;
                // Flatten the PathGeometry
                var flattened = data.GetFlattenedPathGeometry(tolerance, ToleranceType.Absolute);
                this.FigureGeometries = flattened.Figures.Select(x => new FigureGeometry(x, strokeThickness)).ToArray();
            }
        }
    }
}
