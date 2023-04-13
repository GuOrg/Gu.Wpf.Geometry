namespace Gu.Wpf.Geometry.Demo.Converters;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

[ValueConversion(typeof(FrameworkElement), typeof(Rect))]
[MarkupExtensionReturnType(typeof(ElementToBoundsRectConverter))]
public sealed class ElementToBoundsRectConverter : MarkupExtension, IValueConverter
{
    public Type? AncestorType { get; set; }

    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var element = (FrameworkElement)value;
        var parent = element?.Parent as FrameworkElement;
        while (parent != null)
        {
            if (parent.GetType() == this.AncestorType)
            {
                return element!.TransformToVisual(parent)
                              .TransformBounds(new Rect(element.DesiredSize));
            }

            parent = parent.Parent as FrameworkElement;
        }

        throw new InvalidOperationException("Did not find parent.");
    }

    object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotSupportedException($"{nameof(ElementToBoundsRectConverter)} can only be used in OneWay bindings");
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
