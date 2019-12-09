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
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            window.FindButton("None").Invoke();
            app.WaitWhileBusy();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Application.FindExe("Gu.Wpf.Geometry.Demo.exe"));
        }

        [Test]
        public void ZoomUniform()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");

            var zoomButton = window.FindButton("Uniform");
            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,0", contentMatrix.Text);

            window.FindButton("None").Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,0", contentMatrix.Text);
        }

        [Test]
        public void ZoomUniformToFill()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            var zoomButton = window.FindButton("UniformToFill");

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.28667,0,0,1.28667,0,-133.33333", contentMatrix.Text);

            window.FindButton("None").Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.28667,0,0,1.28667,0,-133.33333", contentMatrix.Text);
        }

        [Test]
        public void Increase()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            window.FindButton("Uniform").Invoke();
            app.WaitWhileBusy();
            var zoomButton = window.FindButton("Increase");

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.24,0,0,1.24,7,-124", contentMatrix.Text);

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("2.48,0,0,2.48,-179,-372", contentMatrix.Text);
        }

        [Test]
        public void Decrease()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            window.FindButton("Uniform").Invoke();
            app.WaitWhileBusy();
            var zoomButton = window.FindButton("Decrease");

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.31,0,0,0.31,146.5,62", contentMatrix.Text);

            Assert.AreEqual(true, zoomButton.IsEnabled);
            zoomButton.Click();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.155,0,0,0.155,169.75,93", contentMatrix.Text);
        }

        [Test]
        public void PanFromNone()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            var zoomBox = window.FindGroupBox("Zoombox");
            window.FindButton("None").Invoke();
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.DragHorizontally(MouseButton.Left, zoomBox.Bounds.Center(), 50);
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,50,0", contentMatrix.Text);

            Mouse.DragHorizontally(MouseButton.Left, zoomBox.Bounds.Center(), -50);
            app.WaitWhileBusy();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.DragVertically(MouseButton.Left, zoomBox.Bounds.Center(), 50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,0,50", contentMatrix.Text);

            Mouse.DragVertically(MouseButton.Left, zoomBox.Bounds.Center(), -50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);
        }

        [Test]
        public void PanFromUniform()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            var zoomBox = window.FindGroupBox("Zoombox");
            window.FindButton("Uniform").Invoke();
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,0", contentMatrix.Text);

            Mouse.DragHorizontally(MouseButton.Left, zoomBox.Bounds.Center(), 50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,150,0", contentMatrix.Text);

            Mouse.DragHorizontally(MouseButton.Left, zoomBox.Bounds.Center(), -50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,0", contentMatrix.Text);

            Mouse.DragVertically(MouseButton.Left, zoomBox.Bounds.Center(), 50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,50", contentMatrix.Text);

            Mouse.DragVertically(MouseButton.Left, zoomBox.Bounds.Center(), -50);
            window.WaitUntilResponsive();
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("0.62,0,0,0.62,100,0", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelTopLeft()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopLeft + new Vector(1, 1);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,0,0", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1025,0,0,1.1025,0,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,0,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelTopLeftExplicitWheelZoomFactor()
        {
            using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            window.FindTextBox("WheelZoomFactor").Text = "1.1";
            window.FindButton("None").Click();
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopLeft + new Vector(1, 1);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1,0,0,1.1,0,0", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.21,0,0,1.21,0,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1,0,0,1.1,0,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("Identity", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelTopRight()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.TopRight + new Vector(-1, 1);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-19.3,0", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1025,0,0,1.1025,-39.565,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-19.3,0", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,0,0", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelBottomRight()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.BottomRight + new Vector(-1, -1);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-19.3,-12.4", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1025,0,0,1.1025,-39.565,-25.42", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-19.3,-12.4", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,0,-0", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelBottomLeft()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.BottomLeft + new Vector(1, -1);
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,0,-12.4", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1025,0,0,1.1025,0,-25.42", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,0,-12.4", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,0,-0", contentMatrix.Text);
        }

        [Test]
        public void MouseWheelCenter()
        {
            using var app = Application.AttachOrLaunch("Gu.Wpf.Geometry.Demo.exe", WindowName);
            var window = app.MainWindow;
            var renderSize = window.FindTextBlock("Size");
            var contentMatrix = window.FindTextBlock("ContentMatrix");
            Mouse.Position = window.FindGroupBox("Zoombox").Bounds.Center();
            Assert.AreEqual("Identity", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-9.65,-6.2", contentMatrix.Text);

            Mouse.Scroll(1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.1025,0,0,1.1025,-19.7825,-12.71", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1.05,0,0,1.05,-9.65,-6.2", contentMatrix.Text);

            Mouse.Scroll(-1);
            Assert.AreEqual("386, 248", renderSize.Text);
            Assert.AreEqual("1,0,0,1,0,-0", contentMatrix.Text);
        }

        [Test]
        public void SetsToUniformInCodeBehind()
        {
            using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "ZoomboxContentChanged");
            var window = app.MainWindow;
            Wait.For(TimeSpan.FromMilliseconds(200));
            window.FindButton("Uniform").Invoke();
            var zoomBox = window.FindGroupBox("Zoom");
            var imageSources = window.FindComboBox("ImageSources");
            using var expected = zoomBox.Capture();
            imageSources.SelectedIndex = 1;
            window.FindButton("UniformToFill").Invoke();
            imageSources.SelectedIndex = 0;
            ImageAssert.AreEqual(expected, zoomBox);
        }
    }
}
