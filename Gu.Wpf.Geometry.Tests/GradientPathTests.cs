namespace Gu.Wpf.Geometry.Tests;

using System.Windows;
using System.Windows.Media;

using Gu.Wpf.Geometry;

using NUnit.Framework;

public class GradientPathTests
{
    [TestCase("0,0; 1,0", "0.5, -5.0", "0.5, 5.0")]
    public void CreatePerpendicularBrush(string ls, string esp, string eep)
    {
        var stops = new GradientStopCollection();
        var line = ls.AsLine();
        var actual = GradientPath.CreatePerpendicularBrush(stops, 10, line);
        Assert.AreEqual(Point.Parse(esp), actual.StartPoint);
        Assert.AreEqual(Point.Parse(eep), actual.EndPoint);
    }

    [TestCase("0,0; 1,0", 0, 1, "0,0", "1,0")]
    [TestCase("0,0; 1,0", 0, 2, "0,0", "2,0")]
    [TestCase("0,0; 1,0", 1, 2, "-1,0", "1,0")]
    public void CreateParallelBrush(string ls, double acc, double tot, string esp, string eep)
    {
        var stops = new GradientStopCollection();
        var line = ls.AsLine();
        var actual = GradientPath.CreateParallelBrush(stops, acc, tot, line);
        Assert.AreEqual(esp, actual.StartPoint.ToString("F0"));
        Assert.AreEqual(eep, actual.EndPoint.ToString("F0"));
    }
}
