namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;
    using System.Windows.Media;

    using Gu.Wpf.Geometry;

    using Xunit;

    public class GradientPathTests
    {
        [Theory]
        [InlineData("0,0; 1,0", 10, "0.5, -5.0", "0.5, 5.0")]
        public void CreatePerpendicularBrush(string ls, double width, string esp, string eep)
        {
            var stops = new GradientStopCollection();
            var line = ls.AsLine();
            var actual = GradientPath.CreatePerpendicularBrush(stops, 10, line);
            Assert.Equal(Point.Parse(esp), actual.StartPoint);
            Assert.Equal(Point.Parse(eep), actual.EndPoint);
        }

        [Theory]
        [InlineData("0,0; 1,0", 0, 1, "0,0", "1,0")]
        [InlineData("0,0; 1,0", 0, 2, "0,0", "2,0")]
        [InlineData("0,0; 1,0", 1, 2, "-1,0", "1,0")]
        public void CreateParallelBrush(string ls, double acc, double tot, string esp, string eep)
        {
            var stops = new GradientStopCollection();
            var line = ls.AsLine();
            var actual = GradientPath.CreateParallelBrush(stops, acc, tot, line);
            Assert.Equal(esp, actual.StartPoint.ToString("F0"));
            Assert.Equal(eep, actual.EndPoint.ToString("F0"));
        }
    }
}
