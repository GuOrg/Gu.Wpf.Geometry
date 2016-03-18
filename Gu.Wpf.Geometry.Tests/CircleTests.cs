namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;
    using Xunit;

    public class CircleTests
    {
        [Theory]
        [InlineData("-2,0; 0,0", "0,0; 1", "-1,0")]
        [InlineData("-5,3; 5,3", "0,0; 4", "-2.646,3")]
        [InlineData("-2,0; 2,0", "0,0; 1", "-1,0")]
        [InlineData("-2,1; 0,1", "0,0; 1", "0,1")]
        [InlineData("-2,1; 2,1", "0,0; 1", "0,1")]
        [InlineData("-2,2; 0,2", "0,0; 1", "null")]
        [InlineData("-2,-2; 0,0", "0,0; 1", "-0.71,-0.71")]
        [InlineData("2,2; 0,0", "0,0; 1", "0.71,0.71")]
        [InlineData("-2,2; 0,0", "0,0; 1", "-0.71,0.71")]
        [InlineData("2,-2; 0,0", "0,0; 1", "0.71,-0.71")]
        public void ClosestIntersection(string ls, string cs, string eps)
        {
            var l = Line.Parse(ls);
            var circle = Circle.Parse(cs);
            var expected = eps == "null" ? (Point?) null : Point.Parse(eps);
            var actual = circle.ClosestIntersection(l);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }
    }
}