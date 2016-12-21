#pragma warning disable WPF1011 // Implement INotifyPropertyChanged.
namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(PlacementOptions))]
    public class PlacementOptionsExtension : MarkupExtension
    {
        public HorizontalPlacement Horizontal { get; set; }

        public VerticalPlacement Vertical { get; set; }

        public double Offset { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PlacementOptions(this.Horizontal, this.Vertical, this.Offset);
        }
    }
}