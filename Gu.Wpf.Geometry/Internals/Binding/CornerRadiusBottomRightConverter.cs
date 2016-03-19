namespace Gu.Wpf.Geometry
{
    using System.Windows;

    internal sealed class CornerRadiusBottomRightConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusBottomRightConverter Default = new CornerRadiusBottomRightConverter();

        private CornerRadiusBottomRightConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.BottomRight;
    }
}