namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class BoxBalloonWindowTests
    {
        private const string WindowName = "BoxBalloonWindow";

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

        [TestCase("Auto Auto 0")]
        [TestCase("Auto Auto 5")]
        [TestCase("Auto Auto -5")]
        [TestCase("Center Center 0")]
        [TestCase("Center Center 5")]
        [TestCase("Center Center -5")]
        [TestCase("Left Top 0")]
        [TestCase("Center Top 0")]
        [TestCase("Right Top 0")]
        [TestCase("Left Bottom 0")]
        [TestCase("Center Bottom 0")]
        [TestCase("Right Bottom 0")]
        [TestCase("Auto Center 0")]
        [TestCase("Left Auto 0")]
        [TestCase("Right Auto 0")]
        [TestCase("Auto Top 0")]
        [TestCase("Auto Bottom 0")]
        public static void Renders(string placement)
        {
            if (WinVersion() is { } winVersion)
            {
                using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
                var window = app.MainWindow;
                _ = window.FindListBox("Placements").Select(placement);
                var groupBox = window.FindGroupBox("Render");
                ImageAssert.AreEqual($".\\Images\\BoxBalloonWindow\\{winVersion}\\{placement}.png", groupBox);
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
