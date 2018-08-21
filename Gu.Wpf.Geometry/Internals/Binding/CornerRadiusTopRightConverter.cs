namespace Gu.Wpf.Geometry
{
    using System.Windows;

    [System.Windows.Data.ValueConversion(typeof(CornerRadius), typeof(double))]
    internal sealed class CornerRadiusTopRightConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusTopRightConverter Default = new CornerRadiusTopRightConverter();

        private CornerRadiusTopRightConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.TopRight;
    }
}
