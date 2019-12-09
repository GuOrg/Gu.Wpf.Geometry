namespace Gu.Wpf.Geometry.UiTests
{
    using System.Windows.Media;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class GradientPathWindowTests
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
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
            if (WinVersion() is { } winVersion)
            {
                using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathLineCapsWindow");
                var window = app.MainWindow;
                _ = window.FindComboBox("LineCap").Select(lineCap.ToString());
                _ = window.FindComboBox("GradientMode").Select(gradientMode.ToString());

                ImageAssert.AreEqual($"Images\\GradientPath\\{winVersion}\\LineCap_{gradientMode}_{lineCap}.png", window.FindGroupBox("Path"));
            }
            else
            {
                Assert.Inconclusive("Unknown windows version.");
            }
        }

        private static string WinVersion()
        {
            if (WindowsVersion.IsWindows7())
            {
                return "Win7";
            }

            if (WindowsVersion.IsWindows10())
            {
                return "Win10";
            }

            return null;
        }
    }
}
