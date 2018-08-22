namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Media;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using Application = Gu.Wpf.UiAutomation.Application;

    public sealed class ZoomWindowTests
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"));
        }

        [Test]
        public void ZoomUniform()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                window.FindButton("Uniform").Click();
                Assert.AreEqual("380, 247.56", window.FindTextBlock("Size").Text);
                Assert.AreEqual("0.618977369189262,0,0,0.618977369189262,97.165,-1.4210854715202E-14", window.FindTextBlock("ContentMatrix").Text);
            }
        }

        [Test]
        public void ZoomUniformToFill()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                window.FindButton("UniformToFill").Click();
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

        [Test]
        public void MouseWheelTopLeft()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                var button = window.FindButton("None");
                if (button.IsEnabled)
                {
                    button.Invoke();
                }

                var contentMatrix = window.FindTextBlock("ContentMatrix");
                var image = window.FindFirstDescendant(ControlType.Image);
                var topLeft = image.Bounds.TopLeft();
                Mouse.Position = topLeft;
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                app.WaitWhileBusy();
                app.WaitWhileBusy();
                var matrix = Matrix.Parse(contentMatrix.Text);
                Assert.AreEqual(1.05, matrix.M11, 1E-3);
                Assert.AreEqual(1.05, matrix.M22, 1E-3);
                Assert.AreEqual(0, matrix.OffsetX, 1E-2);
                Assert.AreEqual(0, matrix.OffsetY, 1E-2);
            }
        }

        [Test]
        public void MouseWheelBottomRight()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                var image = window.FindFirstDescendant(ControlType.Image);
                Assert.AreEqual("Identity", contentMatrix.Text);

                var topLeft = image.Bounds.TopLeft();
                var size = Size.Parse(renderSize.Text);
                Mouse.Click(MouseButton.Left, new Point(topLeft.X + size.Width - 1, topLeft.Y + size.Height - 1));
                Mouse.Scroll(1);
                app.WaitWhileBusy();
                app.WaitWhileBusy();
                var matrix = Matrix.Parse(contentMatrix.Text);
                Assert.AreEqual(1.05, matrix.M11, 1E-3);
                Assert.AreEqual(1.05, matrix.M22, 1E-3);
                var expectedOffsetX = -0.05 * size.Width;
                Assert.AreEqual(expectedOffsetX, matrix.OffsetX, 1E-1);
                var expectedOffsetY = -0.05 * size.Height;
                Assert.AreEqual(expectedOffsetY, matrix.OffsetY, 1E-1);
            }
        }
    }
}
