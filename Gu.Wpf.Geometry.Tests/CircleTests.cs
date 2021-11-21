namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;

    using NUnit.Framework;

    public class CircleTests
    {
        [TestCase("-2,0; 0,0", "0,0; 1", "-1,0")]
        [TestCase("-5,3; 5,3", "0,0; 4", "-2.646,3")]
        [TestCase("-2,0; 2,0", "0,0; 1", "-1,0")]
        [TestCase("-2,1; 0,1", "0,0; 1", "0,1")]
        [TestCase("-2,1; 2,1", "0,0; 1", "0,1")]
        [TestCase("-2,2; 0,2", "0,0; 1", "null")]
        [TestCase("-2,-2; 0,0", "0,0; 1", "-0.71,-0.71")]
        [TestCase("2,2; 0,0", "0,0; 1", "0.71,0.71")]
        [TestCase("-2,2; 0,0", "0,0; 1", "-0.71,0.71")]
        [TestCase("2,-2; 0,0", "0,0; 1", "0.71,-0.71")]
        public void ClosestIntersection(string ls, string cs, string eps)
        {
            var l = Line.Parse(ls);
            var circle = Circle.Parse(cs);
            var expected = eps is "null" ? (Point?)null : Point.Parse(eps);
            var actual = circle.ClosestIntersection(l);
            PointAssert.AreEqual(expected, actual, 2);
        }
    }
}
