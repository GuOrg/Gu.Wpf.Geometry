namespace Gu.Wpf.Geometry
{
    using System.Windows;

    internal sealed class CornerRadiusTopLeftConverter : CornerRadiusConverter
    {
        internal static readonly CornerRadiusTopLeftConverter Default = new CornerRadiusTopLeftConverter();

        private CornerRadiusTopLeftConverter()
        {
        }

        protected override double GetRadius(CornerRadius cornerRadius) => cornerRadius.TopLeft;
    }
}