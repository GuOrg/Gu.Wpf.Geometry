namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class GradientPathParallelWindowTests
    {
        private const string WindowName = "GradientPathParallelWindow";

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
            ImageAssert.AreEqual($"Images\\GradientPathParallelWindow\\{TestImage.Current}\\Path.png", window.FindGroupBox("Path"), TestImage.OnFail);
        }
    }
}
