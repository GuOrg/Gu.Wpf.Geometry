namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class GradientPathIssue29WindowTests
    {
        private const string WindowName = "GradientPathIssue29Window";

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched("Gu.Wpf.Geometry.Demo.exe");
        }

        [Test]
        public static void ArcSegmentLargeArcIssue29()
        {
            if (WinVersion() is { } winVersion)
            {
                using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName);
                var window = app.MainWindow;
                ImageAssert.AreEqual($"Images\\GradientPathIssue29Window\\{winVersion}\\ArcSegmentLargeArc.png", window.FindGroupBox("Path"));
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
