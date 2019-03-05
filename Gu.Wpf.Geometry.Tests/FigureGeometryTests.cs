namespace Gu.Wpf.Geometry.Tests
{
    using Gu.Wpf.Geometry;

    using NUnit.Framework;

    public class FigureGeometryTests
    {
        [TestCase("0,0; 1,0", "1,0; 2,0", -10, "0,10; 1,10", "1,10; 2,10")]
        [TestCase("0,0; 2,0", "2,0; 2,2", -1, "0,1; 1,1", "1,1; 1,2")]
        [TestCase("0,0; 2,0", "2,0; 2,2", -2, "0,2; 0,2", "0,2; 0,2")]
        public void CreateOffsetLines(string l1, string l2, double offset, string el1S, string el2S)
        {
            var lines = new[] { l1.AsLine(), l2.AsLine() };
            var actual = GradientPath.FlattenedFigure.CreateOffsetLines(lines, offset);
            Assert.AreEqual(el1S, actual[0].ToString("F0"));
            Assert.AreEqual(el2S, actual[1].ToString("F0"));
        }
    }
}
