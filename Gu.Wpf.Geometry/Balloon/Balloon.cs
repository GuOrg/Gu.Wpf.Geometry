namespace Gu.Wpf.Geometry
{
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    public class Balloon : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(Balloon));

        public static readonly DependencyProperty ConnectorOffsetProperty = BalloonBase.ConnectorOffsetProperty.AddOwner(typeof(Balloon));

        public static readonly DependencyProperty ConnectorAngleProperty = BalloonBase.ConnectorAngleProperty.AddOwner(typeof(Balloon));

        public static readonly DependencyProperty PlacementTargetProperty = BalloonBase.PlacementTargetProperty.AddOwner(typeof(Balloon));

        public static readonly DependencyProperty PlacementOptionsProperty = BalloonBase.PlacementOptionsProperty.AddOwner(typeof(Balloon));

        public static readonly DependencyProperty ForcePopupToRespectClipToBoundsProperty = BalloonBase.ForcePopupToRespectClipToBoundsProperty.AddOwner(typeof(Balloon));

        static Balloon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Balloon), new FrameworkPropertyMetadata(typeof(Balloon)));
        }

        public Balloon()
        {
            this.Loaded += OnLoaded;
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

        public bool ForcePopupToRespectClipToBounds
        {
            get { return (bool)this.GetValue(ForcePopupToRespectClipToBoundsProperty); }
            set { this.SetValue(ForcePopupToRespectClipToBoundsProperty, value); }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var balloon = (Balloon)sender;
            if (balloon.ClipToBounds)
            {
                return;
            }

            if (balloon.ForcePopupToRespectClipToBounds)
            {
                foreach (var ancestor in balloon.Ancestors().OfType<Decorator>())
                {
                    if (ancestor.ClipToBounds)
                    {
                        ancestor.ClipToBounds = false;
                        ancestor.InvalidateVisual();
                        ancestor.Ancestors()
                                .OfType<FrameworkElement>()
                                .FirstOrDefault()?.InvalidateVisual();
                    }
                }
            }
        }
    }
}