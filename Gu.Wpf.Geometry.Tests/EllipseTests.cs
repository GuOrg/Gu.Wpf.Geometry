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

        [Theory]
        [InlineData("0,0; 1; 1", 10, "0.985,0.174")]
        [InlineData("0,0; 2; 3", 0, "2,0")]
        [InlineData("0,0; 2; 3", 90, "0,3")]
        [InlineData("0,0; 2; 3", 10, "1.986,0.35")]
        public void PointAtAngle(string es, double angle, string eps)
        {
            var ellipse = Ellipse.Parse(es);
            var expected = Point.Parse(eps);
            var direction = new Vector(1, 0).Rotate(angle);
            var actual = ellipse.PointOnCircumference(direction);
            Assert.Equal(expected, actual, PointComparer.Default);
            Assert.Equal(ellipse.CenterPoint.DistanceTo(actual), ellipse.RadiusInDirection(direction), 3);
        }
    }
}
