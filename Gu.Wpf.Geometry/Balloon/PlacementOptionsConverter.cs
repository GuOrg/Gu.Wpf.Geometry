namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Security;

    public class PlacementOptionsConverter : TypeConverter
    {
        private static readonly char[] SeparatorChars = { ',', ' ' };

        public override bool CanConvertFrom(
            ITypeDescriptorContext typeDescriptorContext,
            Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(
            ITypeDescriptorContext typeDescriptorContext,
            Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(
            ITypeDescriptorContext typeDescriptorContext,
            CultureInfo cultureInfo,
            object source)
        {
            var text = source as string;
            if (text == null)
            {
                return base.ConvertFrom(typeDescriptorContext, cultureInfo, source);
            }

            try
            {
                var args = text.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
                switch (args.Length)
                {
                    case 1:
                        return ParseOne(args[0], text);
                    case 2:
                        return ParseTwo(args[0], args[1], text);
                    case 3:
                        return ParseThree(args[0], args[1], args[2], text);
                    default:
                        throw FormatException(text);
                }

            }
            catch (Exception e)
            {
                var exception = FormatException(text, e);
                throw exception;
            }
        }

        [SecurityCritical]
        public override object ConvertTo(
            ITypeDescriptorContext typeDescriptorContext,
            CultureInfo cultureInfo,
            object value,
            Type destinationType)
        {
            throw new NotSupportedException();
        }

        private static PlacementOptions ParseOne(string arg, string text)
        {
            if (string.Equals(arg, nameof(HorizontalPlacement.Center), StringComparison.OrdinalIgnoreCase))
            {
                return PlacementOptions.Center;
            }

            throw FormatException(text);
        }

        private static PlacementOptions ParseTwo(string arg1, string arg2, string text)
        {
            double offset;
            if (double.TryParse(arg2, out offset))
            {
                var options = ParseOne(arg1, text);
                return new PlacementOptions(options.Horizontal, options.Vertical, offset);
            }

            HorizontalPlacement horizontal;
            VerticalPlacement vertical;
            if (TryParsePlacements(arg1, arg2, out horizontal, out vertical))
            {
                return new PlacementOptions(horizontal, vertical, 0);
            }

            throw FormatException(text);
        }

        private static PlacementOptions ParseThree(string arg1, string arg2, string arg3, string text)
        {
            try
            {
                var offset = double.Parse(arg3, CultureInfo.InvariantCulture);
                HorizontalPlacement horizontal;
                VerticalPlacement vertical;
                if (TryParsePlacements(arg1, arg2, out horizontal, out vertical))
                {
                    return new PlacementOptions(horizontal, vertical, offset);
                }

                throw FormatException(text);
            }
            catch (Exception e)
            {
                throw FormatException(text, e);
            }
        }


        private static bool TryParsePlacements(string arg1, string arg2, out HorizontalPlacement horizontal, out VerticalPlacement vertical)
        {
            vertical = default(VerticalPlacement);
            return (Enum.TryParse(arg1, true, out horizontal) &&
                    Enum.TryParse(arg2, true, out vertical)) ||
                   (Enum.TryParse(arg2, true, out horizontal) &&
                    Enum.TryParse(arg1, true, out vertical));
        }

        private static FormatException FormatException(string text, Exception inner = null)
        {
            var message = $"Could not parse {nameof(PlacementOptions)} from {text}.\r\n" +
                          $"Expected a string like 'Left Bottom'.\r\n" +
                          $"Valid separators are {{{string.Join(", ", SeparatorChars.Select(x => $"'x'"))}}}";
            return inner != null
                ? new FormatException(message, inner)
                : FormatException(message);
        }
    }
}