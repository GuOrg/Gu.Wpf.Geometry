namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;
    using Xunit;

    public class EllipseTests
    {
        [Theory]
        [InlineData("0,0; 2; 3", "1,0", "2,0")]
        [InlineData("0,0; 2; 3", "0,1", "0,3")]
        [InlineData("0,0; 2; 3", "-1,0", "-2,0")]
        [InlineData("0,0; 2; 3", "0,-1", "0,-3")]
        [InlineData("1,2; 1; 1", "0,1", "1,3")]
        public void PointOnCircumference(string es, string vs, string eps)
        {
            var ellipse = Ellipse.Parse(es);
            var direction = Vector.Parse(vs);
            var expected = Point.Parse(eps);
            var actual = ellipse.PointOnCircumference(direction);
            Assert.Equal(expected, actual, PointComparer.Default);
        }
    }
}
