namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    internal abstract class CornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.GetRadius((CornerRadius) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only supports one way bindings");
        }

        protected abstract double GetRadius(CornerRadius cornerRadius);
    }
}
