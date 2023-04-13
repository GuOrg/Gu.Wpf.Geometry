namespace Gu.Wpf.Geometry.Demo;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Interaction logic for PathDebugView.xaml.
/// </summary>
public partial class PathDebugView : UserControl
{
    public PathDebugView()
    {
        this.InitializeComponent();
    }

    private void OnGradientModeRadioButtonChecked(object sender, RoutedEventArgs args)
    {
        if (this.GradientPath is null)
        {
            return;
        }

        var radioButton = (RadioButton)args.Source;
        this.GradientPath.SetCurrentValue(GradientPath.GradientModeProperty, radioButton.Tag);
    }

    private void OnStartLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
    {
        if (this.GradientPath is null)
        {
            return;
        }

        var radioButton = (RadioButton)args.Source;
        this.GradientPath.SetCurrentValue(GradientPath.StrokeStartLineCapProperty, (PenLineCap)radioButton.Tag);
    }

    private void OnEndLineCapRadioButtonChecked(object sender, RoutedEventArgs args)
    {
        if (this.GradientPath is null)
        {
            return;
        }

        var radioButton = (RadioButton)args.Source;
        this.GradientPath.SetCurrentValue(GradientPath.StrokeEndLineCapProperty, (PenLineCap)radioButton.Tag);
    }

    private void OnColorInterpolationModeRadioButtonChecked(object sender, RoutedEventArgs args)
    {
        if (this.GradientPath is null)
        {
            return;
        }

        var radioButton = (RadioButton)args.Source;
        this.GradientPath.SetCurrentValue(GradientPath.ColorInterpolationModeProperty, (ColorInterpolationMode)radioButton.Tag);
    }
}
