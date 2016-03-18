namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;

    internal static class CornerRadiusExt
    {
        internal static CornerRadius ScaleBy(this CornerRadius cornerRadius, double factor)
        {
            return new CornerRadius(
                factor * cornerRadius.TopLeft,
                factor * cornerRadius.TopRight,
                factor * cornerRadius.BottomRight,
                factor * cornerRadius.BottomLeft);
        }

        internal static CornerRadius InflateBy(this CornerRadius cornerRadius, double value)
        {
            return new CornerRadius(
                value + cornerRadius.TopLeft,
                value + cornerRadius.TopRight,
                value + cornerRadius.BottomRight,
                value + cornerRadius.BottomLeft);
        }

        internal static CornerRadius WithMin(this CornerRadius cornerRadius, double min)
        {
            return new CornerRadius(
                Math.Max(min, cornerRadius.TopLeft),
                Math.Max(min, cornerRadius.TopRight),
                Math.Max(min, cornerRadius.BottomRight),
                Math.Max(min, cornerRadius.BottomLeft));
        }
    }
}
