namespace Gu.Wpf.Geometry.UiTests
{
    using System.IO;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class AngularGradientWindowTests
    {
        private const string WindowName = "AngularGradientWindow";

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
            if (WinVersion() is { } winVersion)
            {
                using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
                var window = app.MainWindow;
                window.FindSlider("StartAngle").Value = startAngle;
                window.FindSlider("CentralAngle").Value = centralAngle;
                var groupBox = window.FindGroupBox("Render");
                ImageAssert.AreEqual($"Images\\AngularGradientWindow\\{winVersion}\\{startAngle} → {centralAngle}.png", groupBox);
            }
            else
            {
                Assert.Inconclusive("Unknown windows version.");
            }

            string WinVersion()
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
}
