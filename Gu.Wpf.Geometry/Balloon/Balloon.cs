namespace Gu.Wpf.Geometry;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Draws a balloon callout.
/// </summary>
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

    /// <summary>
    /// Gets or sets the <see cref="CornerRadius"/> property allows users to control the roundness of the corners independently by
    /// setting a radius value for each corner.  Radius values that are too large are scaled so that they
    /// smoothly blend from corner to corner.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Vector"/> specifying the connector of the <see cref="Balloon"/>.
    /// </summary>
    public Vector ConnectorOffset
    {
        get => (Vector)this.GetValue(ConnectorOffsetProperty);
        set => this.SetValue(ConnectorOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the angle of the connector of the <see cref="Balloon"/>.
    /// </summary>
    public double ConnectorAngle
    {
        get => (double)this.GetValue(ConnectorAngleProperty);
        set => this.SetValue(ConnectorAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets PlacementTarget property of the <see cref="Balloon"/>.
    /// </summary>
    public UIElement? PlacementTarget
    {
        get => (UIElement?)this.GetValue(PlacementTargetProperty);
        set => this.SetValue(PlacementTargetProperty, value);
    }

    /// <summary>
    /// Gets or sets <see cref="PlacementOptions"/> property of the <see cref="Balloon"/>.
    /// </summary>
    public PlacementOptions PlacementOptions
    {
        get => (PlacementOptions)this.GetValue(PlacementOptionsProperty);
        set => this.SetValue(PlacementOptionsProperty, value);
    }
}
