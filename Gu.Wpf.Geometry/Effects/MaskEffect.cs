namespace Gu.Wpf.Geometry.Effects;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

/// <summary>An effect that makes black pixels transparent and all other pixels the provided color.</summary>
public class MaskEffect : ShaderEffect
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
        nameof(Input),
        typeof(MaskEffect),
        0);

    /// <summary>Identifies the <see cref="Color"/> dependency property.</summary>
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(MaskEffect),
        new UIPropertyMetadata(Color.FromArgb(102, 255, 0, 0), PixelShaderConstantCallback(0)));

    /// <summary>Identifies the <see cref="Tolerance"/> dependency property.</summary>
    public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
        nameof(Tolerance),
        typeof(double),
        typeof(MaskEffect),
        new UIPropertyMetadata(0.05D, PixelShaderConstantCallback(1)));

    /// <summary>
    /// The uri should be something like pack://application:,,,/Gu.Wpf.Geometry;component/Effects/MaskEffect.ps
    /// The file MaskEffect.ps should have BuildAction: Resource.
    /// </summary>
    private static readonly PixelShader Shader = new()
    {
        UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/MaskEffect.ps", UriKind.Absolute),
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskEffect"/> class.
    /// </summary>
    public MaskEffect()
    {
        this.PixelShader = Shader;
        this.UpdateShaderValue(InputProperty);
        this.UpdateShaderValue(ColorProperty);
        this.UpdateShaderValue(ToleranceProperty);
    }

    /// <summary>
    /// Gets or sets the Brush called "Input" that is required by a shader.
    /// This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
    /// </summary>
    public Brush Input
    {
        get => (Brush)this.GetValue(InputProperty);
        set => this.SetValue(InputProperty, value);
    }

    /// <summary>Gets or sets the color to use for non-black pixels.</summary>
    public Color Color
    {
        get => (Color)this.GetValue(ColorProperty);
        set => this.SetValue(ColorProperty, value);
    }

    /// <summary>Gets or sets the tolerance for what to treat as black.</summary>
    public double Tolerance
    {
        get => (double)this.GetValue(ToleranceProperty);
        set => this.SetValue(ToleranceProperty, value);
    }
}
