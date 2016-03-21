namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;

    using Xunit;

    public class RayTests
    {
        [Theory]
        [InlineData("-2,0; 1,0", "0,0; 1", "-1,0")]
        [InlineData("2,0; -1,0", "0,0; 1", "1,0")]
        [InlineData("0,2; 0,-1", "0,0; 1", "0,1")]
        [InlineData("0,-2; 0,1", "0,0; 1", "0,-1")]
        [InlineData("-2,0; -1,0", "0,0; 1", "null")]
        public void FirstIntersectionWithCircle(string ls, string cs, string eps)
        {
            var ray = Ray.Parse(ls);
            var circle = Circle.Parse(cs);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = ray.FirstIntersectionWith(circle);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }

        [Theory]
        [InlineData("0,0; 1")]
        [InlineData("1,2; 3")]
        [InlineData("-1,-2; 3")]
        public void FirstIntersectionWithCircleFromInsideRoundtrips(string cs)
        {
            var circle = Circle.Parse(cs);
            var xv = new Vector(1, 0);
            for (var i = -180; i < 180; i++)
            {
                var direction = xv.Rotate(i);
                var pointOnCircumference = circle.PointOnCircumference(direction);
                var ray = new Ray(circle.Center, direction);
                var actual = ray.FirstIntersectionWith(circle);
                Assert.Equal(pointOnCircumference, actual, NullablePointComparer.Default);
            }
        }

        [Theory]
        [InlineData("0,0; 1")]
        [InlineData("1,2; 3")]
        [InlineData("-1,2; 3")]
        [InlineData("-1,-2; 3")]
        public void FirstIntersectionWithCircleFromOutsideRoundtrips(string cs)
        {
            var circle = Circle.Parse(cs);
            var xv = new Vector(1, 0);
            for (var i = -180; i < 180; i++)
            {
                var direction = xv.Rotate(i);
                var pointOnCircumference = circle.PointOnCircumference(direction);
                var ray = new Ray(pointOnCircumference + direction, direction.Negated());
                var actual = ray.FirstIntersectionWith(circle);
                Assert.Equal(pointOnCircumference, actual, NullablePointComparer.Default);
            }
        }

        [Theory]
        [InlineData("-2,0; 1,0", "0,0; 1; 1", "-1,0")]
        [InlineData("0,0; 1,0", "0,0; 1; 1", "1,0")]
        [InlineData("0,0; 1,0", "0,0; 3; 5", "3,0")]
        [InlineData("0,0; -1,0", "0,0; 3; 5", "-3,0")]
        [InlineData("0,0; -1,0", "0,0; 1; 1", "-1,0")]
        [InlineData("-2,1; 1,0", "0,0; 1; 1", "0,1")]
        [InlineData("2,0; -1,0", "0,0; 1; 1", "1,0")]
        [InlineData("0,2; 0,-1", "0,0; 1; 1", "0,1")]
        [InlineData("0,-2; 0,1", "0,0; 1; 1", "0,-1")]
        [InlineData("0,0; 0,1", "0,0; 1; 1", "0,1")]
        [InlineData("0,0; 0,1", "0,0; 2; 3", "0,3")]
        [InlineData("0,0; 0,-1", "0,0; 1; 1", "0,-1")]
        [InlineData("0,0; 0,-1", "0,0; 2; 3", "0,-3")]
        [InlineData("-5,5; 1,-1", "3,5; 5; 3", "2,2")] // got this from CAD
        [InlineData("-8,0; 1,-1", "0,0; 5; 3", "-1,-3")] // got this from CAD
        [InlineData("-2,0; -1,0", "0,0; 1; 1", "null")]
        [InlineData("-2,2; 1,0", "0,0; 1; 1", "null")]
        public void FirstIntersectionWithEllipse(string rs, string es, string eps)
        {
            var ray = Ray.Parse(rs);
            var ellipse = Ellipse.Parse(es);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = ray.FirstIntersectionWith(ellipse);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }

        [Theory]
        [InlineData("0,0; 1; 1")]
        [InlineData("0,0; 2; 3")]
        [InlineData("1,2; 3; 4")]
        [InlineData("-1,-2; 3; 4")]
        public void FirstIntersectionWithEllipseFromInsideRoundtrips(string es)
        {
            var ellipse = Ellipse.Parse(es);
            var xv = new Vector(1, 0);
            for (var i = -180; i < 180; i++)
            {
                var direction = xv.Rotate(i);
                var pointOnCircumference = ellipse.PointOnCircumference(direction);
                var ray = new Ray(ellipse.Center, direction);
                var actual = ray.FirstIntersectionWith(ellipse);
                Assert.Equal(pointOnCircumference, actual, NullablePointComparer.Default);
            }
        }

        [Theory]
        [InlineData("0,0; 1; 1")]
        [InlineData("1,2; 3; 4")]
        [InlineData("-1,-2; 3; 4")]
        public void FirstIntersectionWithEllipseFromOutsideRoundtrips(string es)
        {
            var ellipse = Ellipse.Parse(es);
            var xv = new Vector(1, 0);
            for (var i = -180; i < 180; i++)
            {
                var direction = xv.Rotate(i);
                var pointOnCircumference = ellipse.PointOnCircumference(direction);
                for (var j = -90; j < 90; j++)
                {
                    direction = xv.Rotate(j);
                    var ray = new Ray(pointOnCircumference + direction, direction.Negated());
                    var actual = ray.FirstIntersectionWith(ellipse);
                    Assert.Equal(pointOnCircumference, actual, NullablePointComparer.Default);
                }
            }
        }

        [Theory]
        [InlineData("0 0 1 1")]
        [InlineData("1 2 3 4")]
        public void FirstIntersectionWithRectFromOutsideRoundtrips(string rs)
        {
            var rect = Rect.Parse(rs);
            var xAxis = new Vector(1, 0);
            for (var i = -180; i < 180; i++)
            {
                var direction = xAxis.Rotate(i);
                var fromCenter = new Ray(rect.CenterPoint(), direction);
                var pointOnRect = fromCenter.FirstIntersectionWith(rect).Value;
                var ray = new Ray(pointOnRect + direction, direction.Negated());
                var actual = ray.FirstIntersectionWith(rect);
                Assert.Equal(pointOnRect, actual, NullablePointComparer.Default);
                //var wallNormal = direction.SnapToOrtho();
                //if (wallNormal == null)
                //{
                //    continue;
                //}

                //for (var j = -89; j < 89; j++)
                //{
                //    var rayDirection = wallNormal.Value.Rotate(j);
                //    var ray = new Ray(pointOnRect + rayDirection, rayDirection.Negated());
                //    var actual = ray.FirstIntersectionWith(rect);
                //    Assert.Equal(pointOnRect, actual, NullablePointComparer.Default);
                //}
            }
        }
    }
}
