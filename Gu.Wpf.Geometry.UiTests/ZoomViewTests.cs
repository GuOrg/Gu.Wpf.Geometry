namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using FlaUI.UIA3;
    using Gu.Wpf.Geometry.UiTests.Helpers;
    using NUnit.Framework;
    using Application = FlaUI.Core.Application;

    public class ZoomViewTests
    {
        [Test]
        public void ZoomUniform()
        {
            // Just a smoke test so that everything builds.
            using (var app = Application.Launch(Info.ProcessStartInfo))
            {
                using (var automation = new UIA3Automation())
                {
                    var window = app.GetMainWindow(automation);
                    var tabItem = window.FindFirstDescendant(x => x.ByName("ZoomViewer")).AsTabItem();
                    tabItem.Click();
                    app.WaitWhileBusy();
                    var renderSize = tabItem.FindFirstDescendant(x => x.ByAutomationId("RenderSize")).AsLabel();
                    var contentMatrix = window.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
                    tabItem.FindFirstDescendant("Uniform").AsButton().Click();
                    var size = Size.Parse(renderSize.Text);
                    var expectedScale = Math.Min(size.Width / 300, size.Height / 400);
                    var matrix = Matrix.Parse(contentMatrix.Text);
                    Assert.AreEqual(expectedScale, matrix.M11, 1E-3);
                    Assert.AreEqual(expectedScale, matrix.M22, 1E-3);
                    var expectedOffsetX = size.Width < 300 ? (size.Width - (expectedScale * 300)) / 2 : 0;
                    Assert.AreEqual(expectedOffsetX, matrix.OffsetX, 1E-3);
                    var expectedOffsetY = size.Height < 400 ? (size.Height - (expectedScale * 400)) / 2 : 0;
                    Assert.AreEqual(expectedOffsetY, matrix.OffsetY, 1E-3);
                }
            }
        }

        [Test]
        public void ZoomUniformToFill()
        {
            // Just a smoke test so that everything builds.
            using (var app = Application.Launch(Info.ProcessStartInfo))
            {
                using (var automation = new UIA3Automation())
                {
                    var window = app.GetMainWindow(automation);
                    var tabItem = window.FindFirstDescendant(x => x.ByName("ZoomViewer")).AsTabItem();
                    tabItem.Click();
                    app.WaitWhileBusy();
                    var renderSize = tabItem.FindFirstDescendant(x => x.ByAutomationId("RenderSize")).AsLabel();
                    var contentMatrix = window.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
                    tabItem.FindFirstDescendant("UniformToFill").AsButton().Click();
                    var size = Size.Parse(renderSize.Text);
                    var expectedScale = Math.Max(size.Width / 300, size.Height / 400);
                    var matrix = Matrix.Parse(contentMatrix.Text);
                    Assert.AreEqual(expectedScale, matrix.M11, 1E-3);
                    Assert.AreEqual(expectedScale, matrix.M22, 1E-3);
                    var expectedOffsetX = size.Width < 300 ? (size.Width - (expectedScale * 300)) / 2 : 0;
                    Assert.AreEqual(expectedOffsetX, matrix.OffsetX, 1E-3);
                    var expectedOffsetY = size.Height < 400 ? (size.Height - (expectedScale * 400)) / 2 : 0;
                    Assert.AreEqual(expectedOffsetY, matrix.OffsetY, 1E-3);
                }
            }
        }
    }
}