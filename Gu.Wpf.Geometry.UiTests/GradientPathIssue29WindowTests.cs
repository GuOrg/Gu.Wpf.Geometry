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
            using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            ImageAssert.AreEqual($"Images\\GradientPathIssue29Window\\{ImageFolder.Current}\\ArcSegmentLargeArc.png", window.FindGroupBox("Path"), ImageFolder.AddAttachment);
        }
    }
}
