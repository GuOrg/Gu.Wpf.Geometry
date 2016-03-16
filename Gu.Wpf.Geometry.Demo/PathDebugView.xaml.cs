namespace Gu.Wpf.Geometry.Demo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for PathDebugView.xaml
    /// </summary>
    public partial class PathDebugView : UserControl
    {
        public PathDebugView()
        {
            InitializeComponent();
        }

        void OnGradientModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            var radioButton = args.Source as RadioButton;
            gradientPath.GradientMode = (GradientMode)radioButton.Tag;
        }

        void OnStartLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            var radioButton = args.Source as RadioButton;
            gradientPath.StrokeStartLineCap = (PenLineCap)radioButton.Tag;
        }

        void OnEndLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            var radioButton = args.Source as RadioButton;
            gradientPath.StrokeEndLineCap = (PenLineCap)radioButton.Tag;
        }

        void OnColorInterpolationModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (gradientPath == null)
                return;

            var radioButton = args.Source as RadioButton;
            gradientPath.ColorInterpolationMode = (ColorInterpolationMode)radioButton.Tag;
        }
    }
}
