namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Controls;

    public class BalloonControl : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty ConnectorOffsetProperty = Balloon.ConnectorOffsetProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty ConnectorAngleProperty = Balloon.ConnectorAngleProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty PlacementTargetProperty = Balloon.PlacementTargetProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty PlacementOptionsProperty = Balloon.PlacementOptionsProperty.AddOwner(typeof(BalloonControl));

        static BalloonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonControl), new FrameworkPropertyMetadata(typeof(BalloonControl)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        public Vector ConnectorOffset
        {
            get { return (Vector)this.GetValue(ConnectorOffsetProperty); }
            set { this.SetValue(ConnectorOffsetProperty, value); }
        }

        public double ConnectorAngle
        {
            get { return (double)this.GetValue(ConnectorAngleProperty); }
            set { this.SetValue(ConnectorAngleProperty, value); }
        }

        public UIElement PlacementTarget
        {
            get { return (UIElement)this.GetValue(PlacementTargetProperty); }
            set { this.SetValue(PlacementTargetProperty, value); }
        }

        public PlacementOptions PlacementOptions
        {
            get { return (PlacementOptions)this.GetValue(PlacementOptionsProperty); }
            set { this.SetValue(PlacementOptionsProperty, value); }
        }
    }
}