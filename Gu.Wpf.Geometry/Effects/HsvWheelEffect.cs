namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>
    /// An effect that renders a HSV colour wheel.
    /// </summary>
    public class HsvWheelEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HsvWheelEffect), 0);

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            "InnerRadius",
            typeof(double),
            typeof(HsvWheelEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty InnerSaturationProperty = DependencyProperty.Register(
            "InnerSaturation",
            typeof(double),
            typeof(HsvWheelEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(HsvWheelEffect),
            new UIPropertyMetadata(1D, PixelShaderConstantCallback(2)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/HsvWheelEffectEffect.ps", UriKind.Absolute)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvWheelEffect"/> class.
        /// </summary>
        public HsvWheelEffect()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(InnerRadiusProperty);
            this.UpdateShaderValue(InnerSaturationProperty);
            this.UpdateShaderValue(ValueProperty);
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

        /// <summary>The value.</summary>
        public double Value
        {
            get => (double)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }
    }
}
