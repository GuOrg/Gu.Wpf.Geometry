namespace Gu.Wpf.Geometry.Tests
{
    using System.Threading;
    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public class ZoomboxTests
    {
        [Test]
        public void RespectsMaxZoom()
        {
            var box = new Zoombox { MaxZoom = 2 };
            Assert.AreEqual(true, ZoomCommands.Increase.CanExecute(2.0, box));
            ZoomCommands.Increase.Execute(2.0, box);
            Assert.AreEqual(2, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(2, box.ContentTransform.Matrix.M22);
            Assert.AreEqual(false, ZoomCommands.Increase.CanExecute(2.0, box));
        }

        [Test]
        public void IncreaseZoomIntParameter()
        {
            var box = new Zoombox();
            Assert.AreEqual(true, ZoomCommands.Increase.CanExecute(3, box));
            ZoomCommands.Increase.Execute(3, box);
            Assert.AreEqual(3, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(3, box.ContentTransform.Matrix.M22);
        }

        [Test]
        public void TruncatesToMaxZoom()
        {
            var box = new Zoombox { MaxZoom = 2 };
            Assert.AreEqual(true, ZoomCommands.Increase.CanExecute(3.0, box));
            ZoomCommands.Increase.Execute(3.0, box);
            Assert.AreEqual(2, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(2, box.ContentTransform.Matrix.M22);
            Assert.AreEqual(false, ZoomCommands.Increase.CanExecute(2.0, box));
        }

        [Test]
        public void RespectsMinZoom()
        {
            var box = new Zoombox { MinZoom = 0.5 };
            Assert.AreEqual(true, ZoomCommands.Decrease.CanExecute(2.0, box));
            ZoomCommands.Decrease.Execute(2.0, box);
            Assert.AreEqual(0.5, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(0.5, box.ContentTransform.Matrix.M22);
            Assert.AreEqual(false, ZoomCommands.Decrease.CanExecute(2.0, box));
        }

        [Test]
        public void DecreaseZoomIntParameter()
        {
            var box = new Zoombox();
            Assert.AreEqual(true, ZoomCommands.Decrease.CanExecute(3, box));
            ZoomCommands.Increase.Execute(3, box);
            Assert.AreEqual(3, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(3, box.ContentTransform.Matrix.M22);
        }

        [Test]
        public void TruncatesToMinZoom()
        {
            var box = new Zoombox { MinZoom = 0.5 };
            Assert.AreEqual(true, ZoomCommands.Decrease.CanExecute(3.0, box));
            ZoomCommands.Decrease.Execute(3.0, box);
            Assert.AreEqual(0.5, box.ContentTransform.Matrix.M11);
            Assert.AreEqual(0.5, box.ContentTransform.Matrix.M22);
            Assert.AreEqual(false, ZoomCommands.Decrease.CanExecute(2.0, box));
        }
    }
}
