namespace Gu.Wpf.Geometry.UiTests
{
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Media;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using Application = Gu.Wpf.UiAutomation.Application;

    public sealed class ZoomWindowTests
    {
        [SetUp]
        public void SetUp()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                window.FindButton("None").Invoke();
            }
        }

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
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");

                var zoomButton = window.FindButton("Uniform");
                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("0.6225,0,0,0.6225,99.625,-1.4210854715202E-14", contentMatrix.Text);

                window.FindButton("None").Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("0.6225,0,0,0.6225,99.625,-1.4210854715202E-14", contentMatrix.Text);
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
                var zoomButton = window.FindButton("UniformToFill");

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.28666666666667,0,0,1.28666666666667,0,-132.833333333333", contentMatrix.Text);

                window.FindButton("None").Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.28666666666667,0,0,1.28666666666667,0,-132.833333333333", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelTopLeft()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindFirstDescendant(ControlType.Image).Bounds.TopLeft;
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,0,0", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,0,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,0,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("Identity", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelTopRight()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", "ZoomWindow"))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindFirstDescendant(ControlType.Image).Bounds.TopRight;
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-15,0", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,-30.75,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-15,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1,0,0,1,-1.4210854715202E-14,0", contentMatrix.Text);
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
