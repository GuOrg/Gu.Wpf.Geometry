namespace Gu.Wpf.Geometry.Demo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for PathDebugView.xaml
    /// </summary>
    public partial class PathDebugView : UserControl
    {
        public PathDebugView()
        {
            this.InitializeComponent();
        }

        private void OnGradientModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (this.gradientPath == null)
            {
                return;
            }

            var radioButton = (RadioButton)args.Source;
            this.gradientPath.SetCurrentValue(Wpf.Geometry.GradientPath.GradientModeProperty, radioButton.Tag);
        }

        private void OnStartLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (this.gradientPath == null)
            {
                return;
            }

            var radioButton = (RadioButton)args.Source;
            this.gradientPath.SetCurrentValue(Wpf.Geometry.GradientPath.StrokeStartLineCapProperty, (PenLineCap)radioButton.Tag);
        }

        private void OnEndLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (this.gradientPath == null)
            {
                return;
            }

            var radioButton = (RadioButton)args.Source;
            this.gradientPath.SetCurrentValue(Wpf.Geometry.GradientPath.StrokeEndLineCapProperty, (PenLineCap)radioButton.Tag);
        }

        private void OnColorInterpolationModeRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            if (this.gradientPath == null)
            {
                return;
            }

            var radioButton = (RadioButton)args.Source;
            this.gradientPath.SetCurrentValue(Wpf.Geometry.GradientPath.ColorInterpolationModeProperty, (ColorInterpolationMode)radioButton.Tag);
        }
    }
}
