namespace Gu.Wpf.Geometry.Demo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Gu.Wpf.Geometry;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dot draggedDot;
        Point startPointMouse;
        Point startPointDot;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonDown(args);

            if (args.Source is Dot)
            {
                draggedDot = args.Source as Dot;
                startPointDot = draggedDot.Center;
                startPointMouse = args.GetPosition(this);
                CaptureMouse();
            }
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            if (draggedDot != null)
            {
                Point pointMouse = args.GetPosition(this);
                draggedDot.Center = new Point(startPointDot.X - startPointMouse.X + pointMouse.X,
                                              startPointDot.Y - startPointMouse.Y + pointMouse.Y);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);

            if (draggedDot != null)
            {
                draggedDot = null;
                ReleaseMouseCapture();
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            if (draggedDot != null)
            {
                draggedDot.Center = startPointDot;
                draggedDot = null;
            }

            base.OnLostMouseCapture(args);
        }

        void OnGradientModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            RadioButton radioButton = args.Source as RadioButton;
            gradientPath.GradientMode = (GradientMode)radioButton.Tag;
        }

        void OnStartLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            RadioButton radioButton = args.Source as RadioButton;
            gradientPath.StrokeStartLineCap = (PenLineCap)radioButton.Tag;
        }

        void OnEndLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            RadioButton radioButton = args.Source as RadioButton;
            gradientPath.StrokeEndLineCap = (PenLineCap)radioButton.Tag;
        }

        void OnColorInterpolationModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            RadioButton radioButton = args.Source as RadioButton;
            gradientPath.ColorInterpolationMode = (ColorInterpolationMode)radioButton.Tag;
        }
    }
}
