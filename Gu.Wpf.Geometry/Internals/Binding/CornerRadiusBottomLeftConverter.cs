namespace Gu.Wpf.Geometry
{
    using System.Windows;

    [System.Windows.Data.ValueConversion(typeof(CornerRadius), typeof(double))]
    internal sealed class CornerRadiusBottomLeftConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusBottomLeftConverter Default = new CornerRadiusBottomLeftConverter();

        private CornerRadiusBottomLeftConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.BottomLeft;
    }
}
