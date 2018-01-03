namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class AngularGradientWindowTests
    {
        private const string WindowName = "AngularGradientWindow";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"));
        }

        [Explicit("Fails on AppVeyor for some reason, can't spot a difference in the pics.")]
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
        public void Renders(int startAngle, int centralAngle)
        {
            using (var app = Application.AttachOrLaunch(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"), WindowName))
            {
                var window = app.MainWindow;
                window.FindSlider("StartAngle").Value = startAngle;
                window.FindSlider("CentralAngle").Value = centralAngle;
                var groupBox = window.FindGroupBox("Render");
                ImageAssert.AreEqual($".\\Images\\AngularGradient_StartAngle_{startAngle}_CentralAngle_{centralAngle}.png", groupBox);
            }
        }
    }
}
