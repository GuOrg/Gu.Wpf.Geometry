namespace Gu.Wpf.Geometry.Tests;

using System.Windows;

using NUnit.Framework;

public class LineTests
{
    [TestCase("1,1; 1,2", "0,3; -1,3", "1,1; 1,3")]
    [TestCase("0,0; 1,0", "1,0; 1,1", "0,0; 1,0")]
    [TestCase("0,0; 1,0", "2,0; 2,1", "0,0; 2,0")]
    public void TrimOrExtendEndWith(string l1S, string l2S, string expected)
    {
        var l1 = l1S.AsLine();
        var l2 = l2S.AsLine();
        var trimmed = l1.TrimOrExtendEndWith(l2);
        Assert.AreEqual(expected, trimmed.ToString("F0"));
    }

    [TestCase("1,0; 2,0", "0,0; 1,0", "1,0; 2,0")]
    [TestCase("0,0; 1,0", "1,0; 2,0", "0,0; 1,0")]
    public void TrimOrExtendEndWithCollinear(string l1S, string l2S, string expected)
    {
        var l1 = l1S.AsLine();
        var l2 = l2S.AsLine();
        var trimmed = l1.TrimOrExtendEndWith(l2);
        Assert.AreEqual(expected, trimmed.ToString("F0"));
    }

    [TestCase("-1,0; 1,0", "0,-1; 0,1", "0,0")]
    [TestCase("-1,0; 1,0", "10,-1; 10,1", "null")]
    [TestCase("-1,0; 0,0", "0,-1; 0,1", "0,0")]
    [TestCase("-1,0; 0,0", "0,-1; 0,0", "0,0")]
    [TestCase("-1,0; 0,0", "0,0; 0,1", "0,0")]
    [TestCase("1,1; 1,2", "0,3; -1,3", "null")]
    public void IntersectionPoint(string l1S, string l2S, string expected)
    {
        var l1 = l1S.AsLine();
        var l2 = l2S.AsLine();
        var actual = l1.IntersectWith(l2, mustBeBetweenStartAndEnd: true);
        Assert.AreEqual(expected, actual.ToString("F0"));
        actual = l2.IntersectWith(l1, mustBeBetweenStartAndEnd: true);
        Assert.AreEqual(expected, actual.ToString("F0"));
    }

    [TestCase("1,1; -1,-1", "0,0,10,10", "0,0")]
    [TestCase("1,1; -1,1", "0,0,10,10", "0,1")]
    [TestCase("9,1; 11,1", "0,0,10,10", "10,1")]
    [TestCase("1,9; 1,11", "0,0,10,10", "1,10")]
    [TestCase("1,1; 1,-1", "0,0,10,10", "1,0")]
    public void IntersectWithRectangleStartingInside(string ls, string rs, string eps)
    {
        var l = Line.Parse(ls);
        var rect = Rect.Parse(rs);
        var expected = Point.Parse(eps);
        var actual = l.ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);

        actual = l.Flip().ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);

        var l2 = l.RotateAroundStartPoint(0.01);
        actual = l2.ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);

        var l3 = l.RotateAroundStartPoint(-0.01);
        actual = l3.ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);
    }

    [TestCase("1,0; -1,0", "0,0,10,10", "0,0")]
    [TestCase("1,10; -1,10", "0,0,10,10", "0,10")]
    [TestCase("0,-1; 0,1", "0,0,10,10", "0,0")]
    [TestCase("0,11; 0,9", "0,0,10,10", "0,10")]
    [TestCase("1,11; -1,11", "0,0,10,10", "null")]
    public void IntersectWithRectangleWhenTangent(string ls, string rs, string eps)
    {
        var l = Line.Parse(ls);
        var rect = Rect.Parse(rs);
        var expected = eps is "null" ? (Point?)null : Point.Parse(eps);
        var actual = l.ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);
    }

    [TestCase("-1,-1; 1,1", "0,0,10,10", "0,0")]
    [TestCase("-1,5; 11,5", "0,0,10,10", "0,5")]
    [TestCase("-1,-2; 11,-2", "0,0,10,10", "null")]
    [TestCase("11,5; -1,5", "0,0,10,10", "10,5")]
    [TestCase("-1,0; 1,0", "0,0,10,10", "0,0")]
    [TestCase("1,-1; 1,11", "0,0,10,10", "1,0")]
    [TestCase("-1,11; 1,9", "0,0,10,10", "0,10")]
    public void IntersectWithRectangleStartingOutside(string ls, string rs, string eps)
    {
        var l = Line.Parse(ls);
        var rect = Rect.Parse(rs);
        var expected = eps is "null" ? (Point?)null : Point.Parse(eps);
        var actual = l.ClosestIntersection(rect);
        PointAssert.AreEqual(expected, actual, 2);
    }
}
