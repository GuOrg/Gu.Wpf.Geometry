namespace Gu.Wpf.Geometry.UiTests;

using Gu.Wpf.UiAutomation;

using NUnit.Framework;

public static class GradientPathIssue29WindowTests
{
    private const string WindowName = "GradientPathIssue29Window";

    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
        Application.KillLaunched("Gu.Wpf.Geometry.Demo.exe");
    }

    [Test]
    public static void ArcSegmentLargeArcIssue29()
    {
        using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName);
        var window = app.MainWindow;
        TestImage.AreEqual("GradientPathIssue29Window", $"ArcSegmentLargeArc.png", window.FindGroupBox("Path"));
    }
}
