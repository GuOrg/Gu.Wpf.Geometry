namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>An effect that turns the input into shades of a single color.</summary>
    public class Desaturate : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(Desaturate), 0);

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength",
            typeof(double),
            typeof(Desaturate),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));

        private static readonly PixelShader Shader = new PixelShader { UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/Desaturate.ps", UriKind.Absolute) };

        public Desaturate()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
        }

        /// <summary>There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.</summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>Desaturates an image. the value can be betwen 0 and 1. 0 means the origianl image is returned. 1 means a monochrome image is produced.</summary>
        public double Strength
        {
            get => (double)this.GetValue(StrengthProperty);
            set => this.SetValue(StrengthProperty, value);
        }
    }
}