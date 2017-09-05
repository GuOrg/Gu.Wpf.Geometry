namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Gu.Wpf.Geometry.UiTests.Helpers;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class ZoomViewTests : IDisposable
    {
        private Gu.Wpf.UiAutomation.Application app;
        private TabItem tabItem;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.app?.Dispose();
            this.app = Gu.Wpf.UiAutomation.Application.Launch(Info.ProcessStartInfo);
            var window = this.app.MainWindow;
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
            Mouse.Click(MouseButton.Left, new Point(topLeft.X + size.Width - 1, topLeft.Y + size.Height - 1));
            Mouse.Scroll(1);
            this.app.WaitWhileBusy();
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
            this.app?.Dispose();
            this.app = null;
        }
    }
}