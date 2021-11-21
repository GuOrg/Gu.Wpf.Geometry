namespace Gu.Wpf.Geometry
{
    using System.Windows;

    [System.Windows.Data.ValueConversion(typeof(CornerRadius), typeof(double))]
    internal sealed class CornerRadiusTopLeftConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusTopLeftConverter Default = new CornerRadiusTopLeftConverter();

        private CornerRadiusTopLeftConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.TopLeft;
    }
}
