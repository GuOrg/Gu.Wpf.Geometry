namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>An effect that turns the input into shades of a single color.</summary>
    public class DesaturateEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(DesaturateEffect), 0);

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength",
            typeof(double),
            typeof(DesaturateEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/DesaturateEffect.ps", UriKind.Absolute)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DesaturateEffect"/> class.
        /// </summary>
        public DesaturateEffect()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
        }

        /// <summary>
        /// There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>
        /// Desaturates an image.
        /// The value can be between 0 and 1.
        /// 0 means the original image is returned.
        /// 1 means a monochrome image is produced.
        /// </summary>
        public double Strength
        {
            get => (double)this.GetValue(StrengthProperty);
            set => this.SetValue(StrengthProperty, value);
        }
    }
}