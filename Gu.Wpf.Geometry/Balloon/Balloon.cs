namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Controls;

    public class Balloon : ContentControl
    {
        /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(Balloon));

        /// <summary>Identifies the <see cref="ConnectorOffset"/> dependency property.</summary>
        public static readonly DependencyProperty ConnectorOffsetProperty = BalloonBase.ConnectorOffsetProperty.AddOwner(typeof(Balloon));

        /// <summary>Identifies the <see cref="ConnectorAngle"/> dependency property.</summary>
        public static readonly DependencyProperty ConnectorAngleProperty = BalloonBase.ConnectorAngleProperty.AddOwner(typeof(Balloon));

        /// <summary>Identifies the <see cref="PlacementTarget"/> dependency property.</summary>
        public static readonly DependencyProperty PlacementTargetProperty = BalloonBase.PlacementTargetProperty.AddOwner(typeof(Balloon));

        /// <summary>Identifies the <see cref="PlacementOptions"/> dependency property.</summary>
        public static readonly DependencyProperty PlacementOptionsProperty = BalloonBase.PlacementOptionsProperty.AddOwner(typeof(Balloon));

        static Balloon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Balloon), new FrameworkPropertyMetadata(typeof(Balloon)));
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }

        public Vector ConnectorOffset
        {
            get => (Vector)this.GetValue(ConnectorOffsetProperty);
            set => this.SetValue(ConnectorOffsetProperty, value);
        }

        public double ConnectorAngle
        {
            get => (double)this.GetValue(ConnectorAngleProperty);
            set => this.SetValue(ConnectorAngleProperty, value);
        }

        public UIElement PlacementTarget
        {
            get => (UIElement)this.GetValue(PlacementTargetProperty);
            set => this.SetValue(PlacementTargetProperty, value);
        }

        public PlacementOptions PlacementOptions
        {
            get => (PlacementOptions)this.GetValue(PlacementOptionsProperty);
            set => this.SetValue(PlacementOptionsProperty, value);
        }
    }
}