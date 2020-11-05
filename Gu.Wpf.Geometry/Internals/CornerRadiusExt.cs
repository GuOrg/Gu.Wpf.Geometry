namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    internal static class CornerRadiusExt
    {
        internal static CornerRadius ScaleBy(this CornerRadius cornerRadius, double factor)
        {
            return new CornerRadius(
                topLeft: factor * cornerRadius.TopLeft,
                topRight: factor * cornerRadius.TopRight,
                bottomRight: factor * cornerRadius.BottomRight,
                bottomLeft: factor * cornerRadius.BottomLeft);
        }

        internal static CornerRadius InflateBy(this CornerRadius cornerRadius, double value)
        {
            return new CornerRadius(
                topLeft: value + cornerRadius.TopLeft,
                topRight: value + cornerRadius.TopRight,
                bottomRight: value + cornerRadius.BottomRight,
                bottomLeft: value + cornerRadius.BottomLeft);
        }

        internal static CornerRadius WithMin(this CornerRadius cornerRadius, double min)
        {
            return new CornerRadius(
                topLeft: Math.Max(min, cornerRadius.TopLeft),
                topRight: Math.Max(min, cornerRadius.TopRight),
                bottomRight: Math.Max(min, cornerRadius.BottomRight),
                bottomLeft: Math.Max(min, cornerRadius.BottomLeft));
        }

        internal static bool IsZero(this CornerRadius cornerRadius)
        {
            return cornerRadius.TopLeft <= 0 &&
                   cornerRadius.TopRight <= 0 &&
                   cornerRadius.BottomRight <= 0 &&
                   cornerRadius.BottomLeft <= 0;
        }

        internal static bool IsAnyZero(this CornerRadius cornerRadius)
        {
            return cornerRadius.TopLeft <= 0 ||
                   cornerRadius.TopRight <= 0 ||
                   cornerRadius.BottomRight <= 0 ||
                   cornerRadius.BottomLeft <= 0;
        }

        internal static bool IsAllEqual(this CornerRadius cornerRadius)
        {
            return cornerRadius.TopLeft == cornerRadius.TopRight &&
                   cornerRadius.TopLeft == cornerRadius.BottomLeft &&
                   cornerRadius.TopLeft == cornerRadius.BottomRight;
        }
    }
}
