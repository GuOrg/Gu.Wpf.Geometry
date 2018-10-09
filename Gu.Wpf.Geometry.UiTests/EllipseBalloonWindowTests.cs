namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class EllipseBalloonWindowTests
    {
        private const string WindowName = "EllipseBalloonWindow";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
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
        public void Renders(string placement)
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                _ = window.FindListBox("Placements").Select(placement);
                var groupBox = window.FindGroupBox("Render");
                ImageAssert.AreEqual($".\\Images\\EllipseBalloonWindow_{placement}.png", groupBox);
            }
        }
    }
}
