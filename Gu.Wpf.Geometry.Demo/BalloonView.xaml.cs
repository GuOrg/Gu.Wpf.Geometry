namespace Gu.Wpf.Geometry.Demo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for BalloonView.xaml
    /// </summary>
    public partial class BalloonView : UserControl
    {
        private bool isDragging;
        private Point mouseDragStart;
        private Point dragStartPos;

        public BalloonView()
        {
            this.InitializeComponent();
        }

        private void OnTargetMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = true;
            this.mouseDragStart = e.GetPosition(this);
            this.dragStartPos = new Point(Canvas.GetLeft(this.Target), Canvas.GetTop(this.Target));
            this.CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!this.isDragging)
            {
                return;
            }

            this.isDragging = false;
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (!this.isDragging)
            {
                return;
            }

            var pos = args.GetPosition(this);
            var offset = pos - this.mouseDragStart;
            var newPos = this.dragStartPos + offset;
            this.Target.SetCurrentValue(Canvas.LeftProperty, newPos.X);
            this.Target.SetCurrentValue(Canvas.TopProperty, newPos.Y);
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            this.isDragging = false;
            base.OnLostMouseCapture(args);
        }
    }
}
