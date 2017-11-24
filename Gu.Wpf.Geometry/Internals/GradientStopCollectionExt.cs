namespace Gu.Wpf.Geometry
{
    using System.Windows.Media;

    internal static class GradientStopCollectionExt
    {
        internal static Color GetColorAt(this GradientStopCollection stops, double offset, ColorInterpolationMode colorInterpolationMode)
        {
            if (stops == null || stops.Count == 0)
            {
                return Color.FromArgb(0, 0, 0, 0);
            }

            if (stops.Count == 1)
            {
                return stops[0].Color;
            }

            var lowerOffset = double.MinValue;
            var upperOffset = double.MaxValue;
            var lowerIndex = -1;
            var upperIndex = -1;

            for (var i = 0; i < stops.Count; i++)
            {
                var gradientStop = stops[i];

                if (lowerOffset < gradientStop.Offset && gradientStop.Offset <= offset)
                {
                    lowerOffset = gradientStop.Offset;
                    lowerIndex = i;
                }

                if (upperOffset > gradientStop.Offset && gradientStop.Offset >= offset)
                {
                    upperOffset = gradientStop.Offset;
                    upperIndex = i;
                }
            }

            if (lowerIndex == -1)
            {
                return stops[upperIndex].Color;
            }
            else if (upperIndex == -1)
            {
                return stops[lowerIndex].Color;
            }

            if (lowerIndex == upperIndex)
            {
                return stops[lowerIndex].Color;
            }

            var clr1 = stops[lowerIndex].Color;
            var clr2 = stops[upperIndex].Color;
            var den = upperOffset - lowerOffset;
            var wt1 = (float)((upperOffset - offset) / den);
            var wt2 = (float)((offset - lowerOffset) / den);
            var clr = default(Color);

            switch (colorInterpolationMode)
            {
                case ColorInterpolationMode.SRgbLinearInterpolation:
                    clr = Color.FromArgb(
                        a: (byte)(wt1 * clr1.A + wt2 * clr2.A),
                        r: (byte)(wt1 * clr1.R + wt2 * clr2.R),
                        g: (byte)(wt1 * clr1.G + wt2 * clr2.G),
                        b: (byte)(wt1 * clr1.B + wt2 * clr2.B));
                    break;

                case ColorInterpolationMode.ScRgbLinearInterpolation:
                    clr = clr1 * wt1 + clr2 * wt2;
                    break;
            }

            return clr;
        }
    }
}
