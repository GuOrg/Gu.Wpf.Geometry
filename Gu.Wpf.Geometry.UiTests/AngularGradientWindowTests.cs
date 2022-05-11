namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class AngularGradientWindowTests
    {
        private const string WindowName = "AngularGradientWindow";

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched("Gu.Wpf.Geometry.Demo.exe");
        }

        [TestCase(0, 360)]
        [TestCase(90, 360)]
        [TestCase(-90, 360)]
        [TestCase(180, 360)]
        [TestCase(-180, 360)]
        [TestCase(270, 360)]
        [TestCase(-270, 360)]
        [TestCase(0, 90)]
        [TestCase(90, 90)]
        [TestCase(-90, 90)]
        [TestCase(180, 90)]
        [TestCase(-180, 90)]
        [TestCase(270, 90)]
        [TestCase(-270, 90)]
        [TestCase(0, 270)]
        [TestCase(90, 270)]
        [TestCase(-90, 270)]
        [TestCase(180, 270)]
        [TestCase(-180, 270)]
        [TestCase(270, 270)]
        [TestCase(-270, 270)]
        [TestCase(0, -360)]
        [TestCase(90, -360)]
        [TestCase(-90, -360)]
        [TestCase(180, -360)]
        [TestCase(-180, -360)]
        [TestCase(270, -360)]
        [TestCase(-270, -360)]
        [TestCase(0, -90)]
        [TestCase(90, -90)]
        [TestCase(-90, -90)]
        [TestCase(180, -90)]
        [TestCase(-180, -90)]
        [TestCase(270, -90)]
        [TestCase(-270, -90)]
        [TestCase(0, -270)]
        [TestCase(90, -270)]
        [TestCase(-90, -270)]
        [TestCase(180, -270)]
        [TestCase(-180, -270)]
        [TestCase(270, -270)]
        [TestCase(-270, -270)]
        public static void Renders(int startAngle, int centralAngle)
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            window.FindSlider("StartAngle").Value = startAngle;
            window.FindSlider("CentralAngle").Value = centralAngle;
            var groupBox = window.FindGroupBox("Render");
            TestImage.AreEqual("AngularGradientWindow",$"{startAngle} → {centralAngle}.png", groupBox);
        }
    }
}
