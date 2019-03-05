namespace Gu.Wpf.Geometry
{
    using System.Windows.Media;

    public partial class GradientPath
    {
        internal class FlattenedSegment
        {
            public readonly Line Line;
            public readonly Geometry Geometry;
            public Brush Brush;

            public FlattenedSegment(Line line, Geometry geometry)
            {
                this.Line = line;
                this.Geometry = geometry;
            }
        }
    }
}
