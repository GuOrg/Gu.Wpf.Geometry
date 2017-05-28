namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>An effect that makes black pixels transparent and all other pixels the provided color.</summary>
    public class MaskEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(MaskEffect), 0);

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(MaskEffect), new UIPropertyMetadata(Color.FromArgb(102, 255, 0, 0), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register("Tolerance", typeof(double), typeof(MaskEffect), new UIPropertyMetadata(0.05D, PixelShaderConstantCallback(1)));

        /// <summary>
        /// The uri should be something like pack://application:,,,/Gu.Wpf.Geometry;component/Effects/MaskEffect.ps
        /// The file MaskEffect.ps should have BuildAction: Resource
        /// </summary>
        private static readonly PixelShader Shader = new PixelShader
                                                     {
                                                         UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/MaskEffect.ps", UriKind.Absolute)
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
        /// There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
        /// </summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>The color to use for non-black pixels.</summary>
        public Color Color
        {
            get => (Color)this.GetValue(ColorProperty);
            set => this.SetValue(ColorProperty, value);
        }

        /// <summary>The tolerance for what to treat as black.</summary>
        public double Tolerance
        {
            get => (double)this.GetValue(ToleranceProperty);
            set => this.SetValue(ToleranceProperty, value);
        }
    }
}