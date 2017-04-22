namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>A gradient that changes value with angle.</summary>
    public class AngularGradient : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(AngularGradient), 0);

        public static readonly DependencyProperty CenterPointProperty = DependencyProperty.Register("CenterPoint", typeof(Point), typeof(AngularGradient), new UIPropertyMetadata(new Point(0.5D, 0.5D), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty StartColorProperty = DependencyProperty.Register("StartColor", typeof(Color), typeof(AngularGradient), new UIPropertyMetadata(Color.FromArgb(255, 0, 0, 255), PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty EndColorProperty = DependencyProperty.Register("EndColor", typeof(Color), typeof(AngularGradient), new UIPropertyMetadata(Color.FromArgb(255, 255, 0, 0), PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(AngularGradient), new UIPropertyMetadata(0D, PixelShaderConstantCallback(3)));

        public static readonly DependencyProperty ArcLengthProperty = DependencyProperty.Register("ArcLength", typeof(double), typeof(AngularGradient), new UIPropertyMetadata(360D, PixelShaderConstantCallback(4)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/AngularGradient.ps", UriKind.Absolute)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AngularGradient"/> class.
        /// </summary>
        public AngularGradient()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(CenterPointProperty);
            this.UpdateShaderValue(StartColorProperty);
            this.UpdateShaderValue(EndColorProperty);
            this.UpdateShaderValue(StartAngleProperty);
            this.UpdateShaderValue(ArcLengthProperty);
        }

        /// <summary>
        /// There has to be a property of type Brush called "Input". This property contains the input image and it is usually not set directly - it is set automatically when our effect is applied to a control.
        /// </summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>The center of the gradient. </summary>
        public Point CenterPoint
        {
            get => (Point)this.GetValue(CenterPointProperty);
            set => this.SetValue(CenterPointProperty, value);
        }

        /// <summary>The primary color of the gradient. </summary>
        public Color StartColor
        {
            get => (Color)this.GetValue(StartColorProperty);
            set => this.SetValue(StartColorProperty, value);
        }

        /// <summary>The secondary color of the gradient. </summary>
        public Color EndColor
        {
            get => (Color)this.GetValue(EndColorProperty);
            set => this.SetValue(EndColorProperty, value);
        }

        /// <summary>The starting angle of the gradient, counterclockwise from X-axis</summary>
        public double StartAngle
        {
            get => (double)this.GetValue(StartAngleProperty);
            set => this.SetValue(StartAngleProperty, value);
        }

        /// <summary>The arc length angle of the gradient, counterclockwise from X-axis</summary>
        public double ArcLength
        {
            get => (double)this.GetValue(ArcLengthProperty);
            set => this.SetValue(ArcLengthProperty, value);
        }
    }
}
