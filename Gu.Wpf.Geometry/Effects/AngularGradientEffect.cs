namespace Gu.Wpf.Geometry.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>A gradient that changes value with angle.</summary>
    public class AngularGradientEffect : ShaderEffect
    {
        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(AngularGradientEffect),
            0);

        /// <summary>Identifies the <see cref="StartColor"/> dependency property.</summary>
        public static readonly DependencyProperty StartColorProperty = DependencyProperty.Register(
            nameof(StartColor),
            typeof(Color),
            typeof(AngularGradientEffect),
            new UIPropertyMetadata(
                Color.FromArgb(255, 0, 0, 255),
                PixelShaderConstantCallback(0)));

        /// <summary>Identifies the <see cref="EndColor"/> dependency property.</summary>
        public static readonly DependencyProperty EndColorProperty = DependencyProperty.Register(
            nameof(EndColor),
            typeof(Color),
            typeof(AngularGradientEffect),
            new UIPropertyMetadata(
                Color.FromArgb(0, 0, 0, 255),
                PixelShaderConstantCallback(1)));

        /// <summary>Identifies the <see cref="CenterPoint"/> dependency property.</summary>
        public static readonly DependencyProperty CenterPointProperty = DependencyProperty.Register(
            nameof(CenterPoint),
            typeof(Point),
            typeof(AngularGradientEffect),
            new UIPropertyMetadata(new Point(0.5D, 0.5D), PixelShaderConstantCallback(2)));

        /// <summary>Identifies the <see cref="StartAngle"/> dependency property.</summary>
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            nameof(StartAngle),
            typeof(double),
            typeof(AngularGradientEffect),
            new UIPropertyMetadata(0D, PixelShaderConstantCallback(3)));

        /// <summary>Identifies the <see cref="CentralAngle"/> dependency property.</summary>
        public static readonly DependencyProperty CentralAngleProperty = DependencyProperty.Register(
            nameof(CentralAngle),
            typeof(double),
            typeof(AngularGradientEffect),
            new UIPropertyMetadata(360D, PixelShaderConstantCallback(4)));

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Gu.Wpf.Geometry;component/Effects/AngularGradientEffect.ps", UriKind.Absolute),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AngularGradientEffect"/> class.
        /// </summary>
        public AngularGradientEffect()
        {
            this.PixelShader = Shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StartColorProperty);
            this.UpdateShaderValue(EndColorProperty);
            this.UpdateShaderValue(CenterPointProperty);
            this.UpdateShaderValue(StartAngleProperty);
            this.UpdateShaderValue(CentralAngleProperty);
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

        /// <summary>Gets or sets the primary color of the gradient. </summary>
        public Color StartColor
        {
            get => (Color)this.GetValue(StartColorProperty);
            set => this.SetValue(StartColorProperty, value);
        }

        /// <summary>Gets or sets the secondary color of the gradient. </summary>
        public Color EndColor
        {
            get => (Color)this.GetValue(EndColorProperty);
            set => this.SetValue(EndColorProperty, value);
        }

        /// <summary>Gets or sets the center of the gradient. </summary>
        public Point CenterPoint
        {
            get => (Point)this.GetValue(CenterPointProperty);
            set => this.SetValue(CenterPointProperty, value);
        }

        /// <summary>Gets or sets the starting angle of the gradient, counterclockwise from X-axis.</summary>
        public double StartAngle
        {
            get => (double)this.GetValue(StartAngleProperty);
            set => this.SetValue(StartAngleProperty, value);
        }

        /// <summary>Gets or sets the arc length angle of the gradient, counterclockwise from X-axis.</summary>
        public double CentralAngle
        {
            get => (double)this.GetValue(CentralAngleProperty);
            set => this.SetValue(CentralAngleProperty, value);
        }
    }
}
