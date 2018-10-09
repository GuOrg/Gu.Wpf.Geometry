namespace Gu.Wpf.Geometry.Demo.Converters
{
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(FrameworkElement), typeof(Rect))]
    public sealed class ElementToBoundsRectConverter : IValueConverter
    {
        public static readonly ElementToBoundsRectConverter Default = new ElementToBoundsRectConverter();

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var element = (FrameworkElement)value;
            return element.TransformToVisual(Window.GetWindow(element)).TransformBounds(new Rect(element.DesiredSize));
        }

        object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException($"{nameof(ElementToBoundsRectConverter)} can only be used in OneWay bindings");
        }
    }
}
