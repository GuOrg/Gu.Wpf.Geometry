namespace Gu.Wpf.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    public partial class GradientPath
    {
        private class GradientGeometry
        {
            internal readonly IReadOnlyList<FigureGeometry> FigureGeometries;

            public GradientGeometry(Geometry data, double tolerance, double strokeThickness)
            {
                // Flatten the PathGeometry
                var flattened = data.GetFlattenedPathGeometry(tolerance, ToleranceType.Absolute);
                this.FigureGeometries = flattened.Figures.Select(x => new FigureGeometry(x, strokeThickness)).ToArray();
            }
        }
    }
}
