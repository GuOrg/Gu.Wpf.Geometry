namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Security;

    /// <summary>
    /// A <see cref="TypeConverter"/> for <see cref="PlacementOptions"/>.
    /// </summary>
    public class PlacementOptionsConverter : TypeConverter
    {
        private static readonly char[] SeparatorChars = { ',', ' ' };

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return false;
        }

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text)
            {
                try
                {
                    var args = text.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
                    return args.Length switch
                    {
                        1 => ParseOne(args[0], text),
                        2 => ParseTwo(args[0], args[1], text),
                        3 => ParseThree(args[0], args[1], args[2], text),
                        _ => throw FormatException(text),
                    };
                }
                catch (Exception e)
                {
                    var exception = FormatException(text, e);
                    throw exception;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            throw new NotSupportedException();
        }

        private static PlacementOptions ParseOne(string arg, string text)
        {
            if (double.TryParse(arg, out double offset))
            {
                return new PlacementOptions(HorizontalPlacement.Auto, VerticalPlacement.Auto, offset);
            }

            return ParseCenterOrAuto(arg, text);
        }

        private static PlacementOptions ParseTwo(string arg1, string arg2, string text)
        {
            if (double.TryParse(arg2, out double offset))
            {
                var options = ParseCenterOrAuto(arg1, text);
                return new PlacementOptions(options.Horizontal, options.Vertical, offset);
            }

            if (TryParsePlacements(arg1, arg2, out HorizontalPlacement horizontal, out VerticalPlacement vertical))
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
                if (TryParsePlacements(arg1, arg2, out HorizontalPlacement horizontal, out VerticalPlacement vertical))
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

        private static PlacementOptions ParseCenterOrAuto(string arg, string text)
        {
            if (string.Equals(arg, nameof(HorizontalPlacement.Auto), StringComparison.OrdinalIgnoreCase))
            {
                return PlacementOptions.Auto;
            }

            if (string.Equals(arg, nameof(HorizontalPlacement.Center), StringComparison.OrdinalIgnoreCase))
            {
                return PlacementOptions.Center;
            }

            throw FormatException(text);
        }

        private static bool TryParsePlacements(string arg1, string arg2, out HorizontalPlacement horizontal, out VerticalPlacement vertical)
        {
            vertical = default;
            return (Enum.TryParse(arg1, ignoreCase: true, result: out horizontal) &&
                    Enum.TryParse(arg2, ignoreCase: true, result: out vertical)) ||
                   (Enum.TryParse(arg2, ignoreCase: true, result: out horizontal) &&
                    Enum.TryParse(arg1, ignoreCase: true, result: out vertical));
        }

        private static FormatException FormatException(string text, Exception? inner = null)
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
