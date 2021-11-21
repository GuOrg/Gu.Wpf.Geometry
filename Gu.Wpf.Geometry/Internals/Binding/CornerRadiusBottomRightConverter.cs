namespace Gu.Wpf.Geometry
{
    using System.Windows;

    [System.Windows.Data.ValueConversion(typeof(CornerRadius), typeof(double))]
    internal sealed class CornerRadiusBottomRightConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusBottomRightConverter Default = new CornerRadiusBottomRightConverter();

        private CornerRadiusBottomRightConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.BottomRight;
    }
}
