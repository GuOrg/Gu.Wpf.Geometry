namespace Gu.Wpf.Geometry.UiTests
{
    using System.Windows.Media;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class GradientPathLineCapsWindowTests
    {
        private const string WindowName = "GradientPathLineCapsWindow";

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched("Gu.Wpf.Geometry.Demo.exe");
        }

        [TestCase(GradientMode.Parallel, PenLineCap.Flat)]
        [TestCase(GradientMode.Parallel, PenLineCap.Round)]
        [TestCase(GradientMode.Parallel, PenLineCap.Square)]
        [TestCase(GradientMode.Parallel, PenLineCap.Triangle)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Flat)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Round)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Square)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Triangle)]
        public static void LineCaps(GradientMode gradientMode, PenLineCap lineCap)
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            _ = window.FindComboBox("LineCap").Select(lineCap.ToString());
            _ = window.FindComboBox("GradientMode").Select(gradientMode.ToString());
            ImageAssert.AreEqual($"Images\\GradientPathLineCapsWindow\\{TestImage.Current}\\{gradientMode}_{lineCap}.png", window.FindGroupBox("Path"), TestImage.OnFail);
        }
    }
}
