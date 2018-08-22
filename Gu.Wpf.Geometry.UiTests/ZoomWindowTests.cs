namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Windows;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using Application = Gu.Wpf.UiAutomation.Application;

    public sealed class ZoomWindowTests
    {
        private const string WindowName = "ZoomWindow";

        [SetUp]
        public void SetUp()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
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
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
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
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
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
        public void Increase()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                window.FindButton("Uniform").Invoke();
                var zoomButton = window.FindButton("Increase");

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.245,0,0,1.245,6.24999999999997,-124.5", contentMatrix.Text);

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("2.49,0,0,2.49,-180.5,-373.5", contentMatrix.Text);
            }
        }

        [Test]
        public void Decrease()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                window.FindButton("Uniform").Invoke();
                var zoomButton = window.FindButton("Decrease");

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("0.31125,0,0,0.31125,146.3125,62.25", contentMatrix.Text);

                Assert.AreEqual(true, zoomButton.IsEnabled);
                zoomButton.Click();
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("0.155625,0,0,0.155625,169.65625,93.375", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelTopLeft()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopLeft + new Vector(1, 1);
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
        public void MouseWheelTopLeftExplicitWheelZoomFactor()
        {
            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                window.FindTextBox("WheelZoomFactor").Text = "1.1";
                window.FindButton("None").Click();
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopLeft + new Vector(1, 1);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1,0,0,1.1,0,0", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.21,0,0,1.21,0,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1,0,0,1.1,0,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("Identity", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelTopRight()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopRight + new Vector(-1, 1);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-19.3,0", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,-39.565,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-19.3,0", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1,0,0,1,3.19744231092045E-14,0", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelBottomRight()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.BottomRight + new Vector(-1, -1);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-19.3,-12.45", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,-39.565,-25.5225", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-19.3,-12.45", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1,0,0,1,3.19744231092045E-14,2.8421709430404E-14", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelBottomLeft()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.BottomLeft + new Vector(1, -1);
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,0,-12.45", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,0,-25.5225", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,0,-12.45", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1,0,0,1,0,2.8421709430404E-14", contentMatrix.Text);
            }
        }

        [Test]
        public void MouseWheelCenter()
        {
            using (var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName))
            {
                var window = app.MainWindow;
                var renderSize = window.FindTextBlock("Size");
                var contentMatrix = window.FindTextBlock("ContentMatrix");
                Mouse.Position = window.FindGroupBox("Zoombox").Bounds.Center();
                Assert.AreEqual("Identity", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-9.65000000000001,-6.20000000000002", contentMatrix.Text);

                Mouse.Scroll(1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.1025,0,0,1.1025,-19.7825,-12.71", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1.05,0,0,1.05,-9.65,-6.20000000000003", contentMatrix.Text);

                Mouse.Scroll(-1);
                Assert.AreEqual("386, 249", renderSize.Text);
                Assert.AreEqual("1,0,0,1,1.59872115546023E-14,-1.68753899743024E-14", contentMatrix.Text);
            }
        }

        [Test]
        public void SetsToUniformInCodeBehind()
        {
            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "ZoomboxContentChanged"))
            {
                var window = app.MainWindow;
                Wait.For(TimeSpan.FromMilliseconds(200));
                window.FindButton("Uniform").Invoke();
                var zoomBox = window.FindGroupBox("Zoom");
                var imageSources = window.FindComboBox("ImageSources");
                using (var expected = zoomBox.Capture())
                {
                    imageSources.SelectedIndex = 1;
                    window.FindButton("UniformToFill").Invoke();
                    imageSources.SelectedIndex = 0;
                    ImageAssert.AreEqual(expected, zoomBox);
                }
            }
        }
    }
}
