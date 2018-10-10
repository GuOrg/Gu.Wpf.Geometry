namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>
    /// An effect that renders a HSL colour wheel.
    /// </summary>
    public class HslWheelEffect : ShaderEffect
    {
        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(HslWheelEffect),
            0);

        /// <summary>Identifies the <see cref="InnerRadius"/> dependency property.</summary>
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            nameof(InnerRadius),
            typeof(double),
            typeof(HslWheelEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));

        /// <summary>Identifies the <see cref="InnerSaturation"/> dependency property.</summary>
        public static readonly DependencyProperty InnerSaturationProperty = DependencyProperty.Register(
            nameof(InnerSaturation),
            typeof(double),
            typeof(HslWheelEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(1)));

        /// <summary>Identifies the <see cref="Lightness"/> dependency property.</summary>
        public static readonly DependencyProperty LightnessProperty = DependencyProperty.Register(
            nameof(Lightness),
            typeof(double),
            typeof(HslWheelEffect),
            new UIPropertyMetadata(0.5D, PixelShaderConstantCallback(2)));

        /// <summary>Identifies the <see cref="StartAngle"/> dependency property.</summary>
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            nameof(StartAngle),
            typeof(double),
            typeof(HslWheelEffect),
            new UIPropertyMetadata(
                90D,
                PixelShaderConstantCallback(3)));

        /// <summary>Identifies the <see cref="CentralAngle"/> dependency property.</summary>
        public static readonly DependencyProperty CentralAngleProperty = DependencyProperty.Register(
            nameof(CentralAngle),
            typeof(double),
            typeof(HslWheelEffect),
            new UIPropertyMetadata(
                -360D,
                PixelShaderConstantCallback(4)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/HslWheelEffect.ps", UriKind.Absolute),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HslWheelEffect"/> class.
        /// </summary>
        public HslWheelEffect()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(InnerRadiusProperty);
            this.UpdateShaderValue(InnerSaturationProperty);
            this.UpdateShaderValue(LightnessProperty);
            this.UpdateShaderValue(StartAngleProperty);
            this.UpdateShaderValue(CentralAngleProperty);
        }

        /// <summary>
        /// There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
        /// </summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>The inner radius.</summary>
        public double InnerRadius
        {
            get => (double)this.GetValue(InnerRadiusProperty);
            set => this.SetValue(InnerRadiusProperty, value);
        }

        /// <summary>The inner saturation.</summary>
        public double InnerSaturation
        {
            get => (double)this.GetValue(InnerSaturationProperty);
            set => this.SetValue(InnerSaturationProperty, value);
        }

        /// <summary>The lightness in Hue, Saturation and Lightness.</summary>
        public double Lightness
        {
            get => (double)this.GetValue(LightnessProperty);
            set => this.SetValue(LightnessProperty, value);
        }

        /// <summary>The starting angle of the gradient, clockwise from X-axis.</summary>
        public double StartAngle
        {
            get => (double)this.GetValue(StartAngleProperty);
            set => this.SetValue(StartAngleProperty, value);
        }

        /// <summary>The central angle of the gradient, positive value for clockwise.</summary>
        public double CentralAngle
        {
            get => (double)this.GetValue(CentralAngleProperty);
            set => this.SetValue(CentralAngleProperty, value);
        }
    }
}
