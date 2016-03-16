using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gu.Wpf.Geometry.Demo
{
    /// <summary>
    /// Interaction logic for Dot.xaml
    /// </summary>
    public partial class Dot : UserControl
    {
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center",
                typeof(Point),
                typeof(Dot),
                new PropertyMetadata(new Point(), OnCenterChanged));

        private bool isDragging;

        public Dot()
        {
            InitializeComponent();
        }

        public Point Center
        {
            set { SetValue(CenterProperty, value); }
            get { return (Point)GetValue(CenterProperty); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonDown(args);
            isDragging = true;
            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (!isDragging)
            {
                return;
            }
            var parent = (IInputElement)VisualTreeHelper.GetParent(this);
            var pointMouse = args.GetPosition(parent);
            SetCurrentValue(CenterProperty, pointMouse);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            ReleaseMouseCapture();
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            isDragging = false;
            base.OnLostMouseCapture(args);
        }

        private static void OnCenterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Dot)obj).ellipseGeo.Center = (Point)args.NewValue;
        }
    }
}
