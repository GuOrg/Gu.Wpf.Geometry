namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using Application = Gu.Wpf.UiAutomation.Application;

    public sealed class AngularGradientWindowTests
    {
        private const string WindowName = "AngularGradientWindow";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var app = Application.Launch(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"), WindowName))
            {
                Wait.For(TimeSpan.FromMilliseconds(500));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"));
        }

        [Test]
        public void Renders()
        {
            using (var app = Application.AttachOrLaunch(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"), WindowName))
            {
                var window = app.MainWindow;
                Assert.Inconclusive("Write tests for different start angles & central angles");
            }
        }
    }
}