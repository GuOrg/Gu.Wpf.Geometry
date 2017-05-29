namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Definitions;
    using FlaUI.Core.Input;
    using FlaUI.UIA3;
    using Gu.Wpf.Geometry.UiTests.Helpers;
    using NUnit.Framework;
    using Application = FlaUI.Core.Application;

    public sealed class ZoomViewTests : IDisposable
    {
        private Application app;
        private UIA3Automation automation;
        private TabItem tabItem;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.app?.Dispose();
            this.app = Application.Launch(Info.ProcessStartInfo);
            this.automation?.Dispose();
            this.automation = new UIA3Automation();
            var window = this.app.GetMainWindow(this.automation);
            this.tabItem = window.FindFirstDescendant(x => x.ByName("ZoomViewer")).AsTabItem();
            this.tabItem.Click();
            this.app.WaitWhileBusy();
        }

        [SetUp]
        public void SetUp()
        {
            this.tabItem.FindFirstDescendant("None").AsButton().Click();
            this.app.WaitWhileBusy();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.Dispose();
        }

        [Test]
        public void ZoomUniform()
        {
            var renderSize = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("Size")).AsLabel();
            var contentMatrix = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
            this.tabItem.FindFirstDescendant("Uniform").AsButton().Click();
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

        [Test]
        public void ZoomUniformToFill()
        {
            var renderSize = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("Size")).AsLabel();
            var contentMatrix = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
            this.tabItem.FindFirstDescendant("UniformToFill").AsButton().Click();
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

        [Test]
        public void MouseWheelTopLeft()
        {
            var contentMatrix = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
            var image = this.tabItem.FindFirstDescendant(x => x.ByControlType(ControlType.Image));
            var topLeft = image.Properties.BoundingRectangle.Value.TopLeft();
            Mouse.Position = topLeft;
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            this.app.WaitWhileBusy();
            var matrix = Matrix.Parse(contentMatrix.Text);
            Assert.AreEqual(1.05, matrix.M11, 1E-3);
            Assert.AreEqual(1.05, matrix.M22, 1E-3);
            Assert.AreEqual(0, matrix.OffsetX, 1E-2);
            Assert.AreEqual(0, matrix.OffsetY, 1E-2);
        }

        [Test]
        public void MouseWheelBottomRight()
        {
            var renderSize = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("Size")).AsLabel();
            var contentMatrix = this.tabItem.FindFirstDescendant(x => x.ByAutomationId("ContentMatrix")).AsLabel();
            var image = this.tabItem.FindFirstDescendant(x => x.ByControlType(ControlType.Image));
            Assert.AreEqual("Identity", contentMatrix.Text);

            var topLeft = image.Properties.BoundingRectangle.Value.TopLeft();
            var size = Size.Parse(renderSize.Text);
            Mouse.Click(MouseButton.Left, new FlaUI.Core.Shapes.Point(topLeft.X + size.Width, topLeft.Y + size.Height));
            Mouse.Scroll(1);
            this.app.WaitWhileBusy();
            var matrix = Matrix.Parse(contentMatrix.Text);
            Assert.AreEqual(1.05, matrix.M11, 1E-3);
            Assert.AreEqual(1.05, matrix.M22, 1E-3);
            var expectedOffsetX = -0.05 * size.Width;
            Assert.AreEqual(expectedOffsetX, matrix.OffsetX, 1E-1);
            var expectedOffsetY = -0.05 * size.Height;
            Assert.AreEqual(expectedOffsetY, matrix.OffsetY, 1E-1);
        }

        public void Dispose()
        {
            try
            {
                this.automation?.Dispose();
                this.automation = null;
                this.app?.Dispose();
                this.app = null;
            }
            catch (Exception)
            {
                // Swallow
            }
        }
    }
}