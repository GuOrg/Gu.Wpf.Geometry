namespace Gu.Wpf.Geometry.Tests
{
    using Gu.Wpf.Geometry;

    using Xunit;

    public class FigureGeometryTests
    {
        [Theory]
        [InlineData("0,0; 1,0", "1,0; 2,0", -10, "0,10; 1,10", "1,10; 2,10")]
        [InlineData("0,0; 2,0", "2,0; 2,2", -1, "0,1; 1,1", "1,1; 1,2")]
        [InlineData("0,0; 2,0", "2,0; 2,2", -2, "0,2; 0,2", "0,2; 0,2")]
        public void CreateOffsetLines(string l1, string l2, double offset, string el1S, string el2S)
        {
            var lines = new[] { l1.AsLine(), l2.AsLine() };
            var actual = GradientPath.FigureGeometry.CreateOffsetLines(lines, offset);
            Assert.Equal(el1S, actual[0].ToString("F0"));
            Assert.Equal(el2S, actual[1].ToString("F0"));
        }
    }
}
