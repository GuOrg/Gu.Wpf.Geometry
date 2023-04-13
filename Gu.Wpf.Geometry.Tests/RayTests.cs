namespace Gu.Wpf.Geometry.Tests;

using System;
using System.Diagnostics;
using System.Windows;

using NUnit.Framework;

public static class RayTests
{
    [TestCase("-2,0; 1,0", "0,0; 1", "-1,0")]
    [TestCase("2,0; -1,0", "0,0; 1", "1,0")]
    [TestCase("0,2; 0,-1", "0,0; 1", "0,1")]
    [TestCase("0,-2; 0,1", "0,0; 1", "0,-1")]
    [TestCase("-2,0; -1,0", "0,0; 1", "null")]
    public static void FirstIntersectionWithCircle(string ls, string cs, string eps)
    {
        var ray = Ray.Parse(ls);
        var circle = Circle.Parse(cs);
        var expected = eps is "null" ? (Point?)null : Point.Parse(eps);
        var actual = ray.FirstIntersectionWith(circle);
        PointAssert.AreEqual(expected, actual, 2);
    }

    [TestCase("0,0; 1")]
    [TestCase("1,2; 3")]
    [TestCase("-1,-2; 3")]
    public static void FirstIntersectionWithCircleFromInsideRoundtrips(string cs)
    {
        var circle = Circle.Parse(cs);
        var xv = new Vector(1, 0);
        for (var i = -180; i < 180; i++)
        {
            var direction = xv.Rotate(i);
            var pointOnCircumference = circle.PointOnCircumference(direction);
            var ray = new Ray(circle.Center, direction);
            var actual = ray.FirstIntersectionWith(circle);
            PointAssert.AreEqual(pointOnCircumference, actual, 2);
        }
    }

    [TestCase("0,0; 1")]
    [TestCase("1,2; 3")]
    [TestCase("-1,2; 3")]
    [TestCase("-1,-2; 3")]
    public static void FirstIntersectionWithCircleFromOutsideRoundtrips(string cs)
    {
        var circle = Circle.Parse(cs);
        var xv = new Vector(1, 0);
        for (var i = -180; i < 180; i++)
        {
            var direction = xv.Rotate(i);
            var pointOnCircumference = circle.PointOnCircumference(direction);
            var ray = new Ray(pointOnCircumference + direction, direction.Negated());
            var actual = ray.FirstIntersectionWith(circle);
            PointAssert.AreEqual(pointOnCircumference, actual, 2);
        }
    }

    [TestCase("-2,0; 1,0", "0,0; 1; 1", "-1,0")]
    [TestCase("0,0; 1,0", "0,0; 1; 1", "1,0")]
    [TestCase("0,0; 1,0", "0,0; 3; 5", "3,0")]
    [TestCase("0,0; -1,0", "0,0; 3; 5", "-3,0")]
    [TestCase("0,0; -1,0", "0,0; 1; 1", "-1,0")]
    [TestCase("-2,1; 1,0", "0,0; 1; 1", "0,1")]
    [TestCase("2,0; -1,0", "0,0; 1; 1", "1,0")]
    [TestCase("0,2; 0,-1", "0,0; 1; 1", "0,1")]
    [TestCase("0,-2; 0,1", "0,0; 1; 1", "0,-1")]
    [TestCase("0,0; 0,1", "0,0; 1; 1", "0,1")]
    [TestCase("0,0; 0,1", "0,0; 2; 3", "0,3")]
    [TestCase("0,0; 0,-1", "0,0; 1; 1", "0,-1")]
    [TestCase("0,0; 0,-1", "0,0; 2; 3", "0,-3")]
    [TestCase("-5,8; 1,-1", "1,2; 3; 4", "-1.4,4.4")] // got this from CAD
    [TestCase("-2,1; 1,0", "0,1; 1; 1", "-1,1")]
    [TestCase("0,3; 0,-1", "0,1; 1; 1", "0,2")]
    [TestCase("0,-1; 0,1", "0,1; 1; 1", "0,0")]
    [TestCase("-2,0; -1,0", "0,0; 1; 1", "null")]
    [TestCase("-2,2; 1,0", "0,0; 1; 1", "null")]
    public static void FirstIntersectionWithEllipse(string rs, string es, string eps)
    {
        var ray = Ray.Parse(rs);
        var ellipse = Ellipse.Parse(es);
        var expected = eps is "null" ? (Point?)null : Point.Parse(eps);
        var actual = ray.FirstIntersectionWith(ellipse);
        PointAssert.AreEqual(expected, actual, 2);
    }

    [TestCase("0,0; 1; 1")]
    [TestCase("0,0; 2; 3")]
    [TestCase("1,2; 3; 4")]
    [TestCase("-1,-2; 3; 4")]
    public static void FirstIntersectionWithEllipseFromInsideRoundtrips(string es)
    {
        var ellipse = Ellipse.Parse(es);
        var xv = new Vector(1, 0);
        for (var i = -180; i < 180; i++)
        {
            var direction = xv.Rotate(i);
            var expected = ellipse.PointOnCircumference(direction);
            var ray = new Ray(ellipse.CenterPoint, direction);
            var actual = ray.FirstIntersectionWith(ellipse);
            PointAssert.AreEqual(expected, actual, 2);
        }
    }

    [TestCase("0,0; 1; 1")]
    [TestCase("1,2; 3; 4")]
    [TestCase("-1,-2; 3; 4")]
    public static void FirstIntersectionWithEllipseFromOutsideRoundtrips(string es)
    {
        var ellipse = Ellipse.Parse(es);
        var xv = new Vector(1, 0);
        for (var i = -180; i < 180; i++)
        {
            var fromCenterDirection = xv.Rotate(i);
            var pointOnCircumference = ellipse.PointOnCircumference(fromCenterDirection);
            var ray = new Ray(pointOnCircumference + fromCenterDirection, fromCenterDirection.Negated());
            var actual = ray.FirstIntersectionWith(ellipse);
            PointAssert.AreEqual(pointOnCircumference, actual, 2);
            for (var j = -70; j < 70; j++)
            {
                var direction = fromCenterDirection.Rotate(j);
                ray = new Ray(pointOnCircumference + direction, direction.Negated());
                actual = ray.FirstIntersectionWith(ellipse);
                PointAssert.AreEqual(pointOnCircumference, actual, 2);
            }
        }
    }

    [TestCase("0 0 1 1")]
    [TestCase("1 2 3 4")]
    public static void FirstIntersectionWithRectFromOutsideRoundtrips(string rs)
    {
        var rect = Rect.Parse(rs);
        var xAxis = new Vector(1, 0);
        for (var i = -180; i < 180; i++)
        {
            var direction = xAxis.Rotate(i);
            var fromCenter = new Ray(rect.CenterPoint(), direction);
            var pointOnRect = fromCenter.FirstIntersectionWith(rect).GetValueOrDefault();
            var ray = new Ray(pointOnRect + direction, direction.Negated());
            var actual = ray.FirstIntersectionWith(rect);
            PointAssert.AreEqual(pointOnRect, actual, 2);

            if (rect.ClosestCornerPoint(pointOnRect)
                    .DistanceTo(pointOnRect) < 0.01)
            {
                continue;
            }

            Vector wallNormal;
            if (Math.Abs(pointOnRect.X - rect.Left) < Constants.Tolerance)
            {
                wallNormal = new Vector(-1, 0);
            }
            else if (Math.Abs(pointOnRect.X - rect.Right) < Constants.Tolerance)
            {
                wallNormal = new Vector(1, 0);
            }
            else if (Math.Abs(pointOnRect.Y - rect.Bottom) < Constants.Tolerance)
            {
                wallNormal = new Vector(0, 1);
            }
            else
            {
                wallNormal = new Vector(0, -1);
            }

            for (var j = -89; j < 89; j++)
            {
                var rayDirection = wallNormal.Rotate(j);
                ray = new Ray(pointOnRect + rayDirection, rayDirection.Negated());
                actual = ray.FirstIntersectionWith(rect);
                if (!NullablePointComparer.TwoDigits.Equals(pointOnRect, actual))
                {
                    Debugger.Break();
                }

                PointAssert.AreEqual(pointOnRect, actual, 2);
            }
        }
    }
}
