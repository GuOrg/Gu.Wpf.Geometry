namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows.Markup;

    /// <summary>
    /// <see cref="MarkupExtension"/> for creating <see cref="PlacementOptions"/>.
    /// </summary>
    [MarkupExtensionReturnType(typeof(PlacementOptions))]
    public class PlacementOptionsExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="HorizontalPlacement"/>.
        /// </summary>
        public HorizontalPlacement Horizontal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VerticalPlacement"/>.
        /// </summary>
        public VerticalPlacement Vertical { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public double Offset { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PlacementOptions(this.Horizontal, this.Vertical, this.Offset);
        }
    }
}
