namespace Gu.Wpf.Geometry.UiTests
{
    using System.Windows.Media;
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
                ImageAssert.AreEqual(".\\Images\\GradientPathParallel.png", window.FindGroupBox("Path"));
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
                ImageAssert.AreEqual(".\\Images\\GradientPathPerpendicular.png", window.FindGroupBox("Path"));
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
                ImageAssert.AreEqual(".\\Images\\GradientPathWithArcSegment.png", window.FindGroupBox("Path"));
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
                ImageAssert.AreEqual(".\\Images\\GradientPathArcSegmentLargeArc.png", window.FindGroupBox("Path"));
            }
        }

        [TestCase(GradientMode.Parallel, PenLineCap.Flat)]
        [TestCase(GradientMode.Parallel, PenLineCap.Round)]
        [TestCase(GradientMode.Parallel, PenLineCap.Square)]
        [TestCase(GradientMode.Parallel, PenLineCap.Triangle)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Flat)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Round)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Square)]
        [TestCase(GradientMode.Perpendicular, PenLineCap.Triangle)]
        public void LineCaps(GradientMode gradientMode, PenLineCap lineCap)
        {
            if (Env.IsAppVeyor)
            {
                return;
            }

            using (var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe", "GradientPathLineCapsWindow"))
            {
                var window = app.MainWindow;
                _ = window.FindComboBox("LineCap").Select(lineCap.ToString());
                _ = window.FindComboBox("GradientMode").Select(gradientMode.ToString());

                ImageAssert.AreEqual($".\\Images\\GradientPathLineCap_{gradientMode}_{lineCap}.png", window.FindGroupBox("Path"));
            }
        }
    }
}
