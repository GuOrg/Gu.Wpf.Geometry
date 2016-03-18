namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;
    using Xunit;

    public class LineTests
    {
        [Theory]
        [InlineData("1,1; 1,2", "0,3; -1,3", "1,1; 1,3")]
        [InlineData("1,1; 1,2", "0,3; -1,3", "1,1; 1,3")]
        [InlineData("0,0; 1,0", "1,0; 2,0", "0,0; 1,0")]
        [InlineData("1,0; 2,0", "0,0; 1,0", "1,0; 2,0")]
        public void TrimOrExtendEndWith(string l1s, string l2s, string expected)
        {
            var l1 = l1s.AsLine();
            var l2 = l2s.AsLine();
            var trimmed = l1.TrimOrExtendEndWith(l2);
            Assert.Equal(expected, trimmed.ToString("F0"));
        }

        [Theory]
        [InlineData("-1,0; 1,0", "0,-1; 0,1", "0,0")]
        [InlineData("-1,0; 1,0", "10,-1; 10,1", "null")]
        [InlineData("-1,0; 0,0", "0,-1; 0,1", "0,0")]
        [InlineData("-1,0; 0,0", "0,-1; 0,0", "0,0")]
        [InlineData("-1,0; 0,0", "0,0; 0,1", "0,0")]
        [InlineData("1,1; 1,2", "0,3; -1,3", "null")]
        public void IntersectionPoint(string l1s, string l2s, string expected)
        {
            var l1 = l1s.AsLine();
            var l2 = l2s.AsLine();
            var actual = l1.IntersectWith(l2);
            Assert.Equal(expected, actual.ToString("F0"));
            actual = l2.IntersectWith(l1);
            Assert.Equal(expected, actual.ToString("F0"));
        }

        [Theory]
        [InlineData("1,1; -1,-1", "0,0,10,10", "0,0")]
        [InlineData("1,1; -1,1", "0,0,10,10", "0,1")]
        [InlineData("9,1; 11,1", "0,0,10,10", "10,1")]
        [InlineData("1,9; 1,11", "0,0,10,10", "1,10")]
        [InlineData("1,1; 1,-1", "0,0,10,10", "1,0")]
        public void IntersectWithRectangleStartingInside(string ls, string rs, string eps)
        {
            var l = Line.Parse(ls);
            var rect = Rect.Parse(rs);
            var expected = Point.Parse(eps);
            var actual = l.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);

            actual = l.Flip().ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);

            var l2 = l.RotateAroundStartPoint(0.01);
            actual = l2.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);

            var l3 = l.RotateAroundStartPoint(-0.01);
            actual = l3.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }

        [Theory]
        [InlineData("1,0; -1,0", "0,0,10,10", "0,0")]
        [InlineData("1,10; -1,10", "0,0,10,10", "0,10")]
        [InlineData("0,-1; 0,1", "0,0,10,10", "0,0")]
        [InlineData("0,11; 0,9", "0,0,10,10", "0,10")]
        [InlineData("1,11; -1,11", "0,0,10,10", "null")]
        public void IntersectWithRectangleWhenTangent(string ls, string rs, string eps)
        {
            var l = Line.Parse(ls);
            var rect = Rect.Parse(rs);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = l.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }

        [Theory]
        [InlineData("-1,-1; 1,1", "0,0,10,10", "0,0")]
        [InlineData("-1,5; 11,5", "0,0,10,10", "0,5")]
        [InlineData("-1,-2; 11,-2", "0,0,10,10", "null")]
        [InlineData("11,5; -1,5", "0,0,10,10", "10,5")]
        [InlineData("-1,0; 1,0", "0,0,10,10", "0,0")]
        [InlineData("1,-1; 1,11", "0,0,10,10", "1,0")]
        [InlineData("-1,11; 1,9", "0,0,10,10", "0,10")]
        public void IntersectWithRectangleStartingOutside(string ls, string rs, string eps)
        {
            var l = Line.Parse(ls);
            var rect = Rect.Parse(rs);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = l.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);

            var l2 = l.RotateAroundStartPoint(0.01);
            actual = l2.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);

            var l3 = l.RotateAroundStartPoint(-0.01);
            actual = l3.ClosestIntersection(rect);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }
    }
}
