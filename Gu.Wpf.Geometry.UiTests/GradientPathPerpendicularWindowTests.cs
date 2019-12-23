namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class GradientPathPerpendicularWindowTests
    {
        private const string WindowName = "GradientPathPerpendicularWindow";

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched("Gu.Wpf.Geometry.Demo.exe");
        }

        [Test]
        public static void Renders()
        {
            using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            ImageAssert.AreEqual($"Images\\GradientPathPerpendicularWindow\\{TestImage.Current}\\Path.png", window.FindGroupBox("Path"), TestImage.OnFail);
        }
    }
}
