namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class GradientPathWindowTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
        }

        [Test]
        public void RendersParallel()
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathParallelWindow"))
            {
                var window = app.MainWindow;
                ImageAssert.AreEqual("GradientPathParallel.png", window.FindGroupBox("Path"));
            }
        }

        [Test]
        public void RendersPerpendicular()
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathPerpendicularWindow"))
            {
                var window = app.MainWindow;
                ImageAssert.AreEqual("GradientPathPerpendicular.png", window.FindGroupBox("Path"));
            }
        }

        [Test]
        public void RendersWhenArcSegmentIssue28()
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathIssue28Window"))
            {
                var window = app.MainWindow;
                ImageAssert.AreEqual("GradientPathWithArcSegment.png", window.FindGroupBox("Path"));
            }
        }

        [Test]
        public void RendersArcSegmentLargeArcIssue29()
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathIssue29Window"))
            {
                var window = app.MainWindow;
                ImageAssert.AreEqual("GradientPathArcSegmentLargeArc.png", window.FindGroupBox("Path"));
            }
        }
    }
}
