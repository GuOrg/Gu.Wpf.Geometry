namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>Fade to a colour by animating the strength.</summary>
    public class FadeEffect : ShaderEffect
    {
        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(FadeEffect),
            0);

        /// <summary>Identifies the <see cref="Strength"/> dependency property.</summary>
        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            nameof(Strength),
            typeof(double),
            typeof(FadeEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));

        /// <summary>Identifies the <see cref="To"/> dependency property.</summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            nameof(To),
            typeof(Color),
            typeof(FadeEffect),
            new UIPropertyMetadata(Color.FromArgb(255, 0, 0, 0), PixelShaderConstantCallback(2)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/FadeEffect.ps", UriKind.Absolute),
        };

        public FadeEffect()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
            this.UpdateShaderValue(ToProperty);
        }

        /// <summary>
        /// There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
        /// </summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>The color used to tint the input.</summary>
        public double Strength
        {
            get => (double)this.GetValue(StrengthProperty);
            set => this.SetValue(StrengthProperty, value);
        }

        /// <summary>The colour to fade to.</summary>
        public Color To
        {
            get => (Color)this.GetValue(ToProperty);
            set => this.SetValue(ToProperty, value);
        }
    }
}
