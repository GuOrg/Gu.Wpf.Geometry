#pragma warning disable WPF0005 // Name of PropertyChangedCallback should match registered name.
namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class GradientPath : FrameworkElement
    {
        /// <summary>Identifies the <see cref="Data"/> dependency property.</summary>
        public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGeometryChanged()));

        /// <summary>Identifies the <see cref="GradientStops"/> dependency property.</summary>
        public static readonly DependencyProperty GradientStopsProperty = GradientBrush.GradientStopsProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGradientChanged()));

        /// <summary>Identifies the <see cref="GradientMode"/> dependency property.</summary>
        public static readonly DependencyProperty GradientModeProperty = DependencyProperty.Register(
            nameof(GradientMode),
            typeof(GradientMode),
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                GradientMode.Perpendicular,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGradientChanged()));

        /// <summary>Identifies the <see cref="ColorInterpolationMode"/> dependency property.</summary>
        public static readonly DependencyProperty ColorInterpolationModeProperty = GradientBrush.ColorInterpolationModeProperty.AddOwner(
                typeof(GradientPath),
                new FrameworkPropertyMetadata(
                    ColorInterpolationMode.SRgbLinearInterpolation,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((GradientPath)d).OnGradientChanged()));

        /// <summary>Identifies the <see cref="StrokeThickness"/> dependency property.</summary>
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                1.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGeometryChanged()));

        /// <summary>Identifies the <see cref="StrokeStartLineCap"/> dependency property.</summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty = Shape.StrokeStartLineCapProperty.AddOwner(
                typeof(GradientPath),
                new FrameworkPropertyMetadata(
                    PenLineCap.Flat,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((GradientPath)d).OnGeometryChanged()));

        /// <summary>Identifies the <see cref="StrokeEndLineCap"/> dependency property.</summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty = Shape.StrokeEndLineCapProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                PenLineCap.Flat,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGeometryChanged()));

        /// <summary>Identifies the <see cref="Tolerance"/> dependency property.</summary>
        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            nameof(Tolerance),
            typeof(double),
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                Geometry.StandardFlatteningTolerance,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGeometryChanged()));

        private readonly Pen pen = new Pen();
        private IReadOnlyList<FlattenedFigure>? flattenedFigures;

        public GradientPath()
        {
            this.GradientStops = new GradientStopCollection();
        }

        public Geometry Data
        {
            get => (Geometry)this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public GradientStopCollection GradientStops
#pragma warning restore CA2227 // Collection properties should be read only
        {
            get => (GradientStopCollection)this.GetValue(GradientStopsProperty);
            set => this.SetValue(GradientStopsProperty, value);
        }

        public GradientMode GradientMode
        {
            get => (GradientMode)this.GetValue(GradientModeProperty);
            set => this.SetValue(GradientModeProperty, value);
        }

        public ColorInterpolationMode ColorInterpolationMode
        {
            get => (ColorInterpolationMode)this.GetValue(ColorInterpolationModeProperty);
            set => this.SetValue(ColorInterpolationModeProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)this.GetValue(StrokeThicknessProperty);
            set => this.SetValue(StrokeThicknessProperty, value);
        }

        public PenLineCap StrokeStartLineCap
        {
            get => (PenLineCap)this.GetValue(StrokeStartLineCapProperty);
            set => this.SetValue(StrokeStartLineCapProperty, value);
        }

        public PenLineCap StrokeEndLineCap
        {
            get => (PenLineCap)this.GetValue(StrokeEndLineCapProperty);
            set => this.SetValue(StrokeEndLineCapProperty, value);
        }

        public double Tolerance
        {
            get => (double)this.GetValue(ToleranceProperty);
            set => this.SetValue(ToleranceProperty, value);
        }

        internal static LinearGradientBrush CreatePerpendicularBrush(GradientStopCollection gradientStops, double strokeThickness, Line line)
        {
            var mp = line.MidPoint;
            var v = line.PerpendicularDirection;
            var startPoint = mp.WithOffset(v, strokeThickness / 2);
            var endPoint = mp.WithOffset(v, -strokeThickness / 2);
            return new LinearGradientBrush(gradientStops, startPoint, endPoint)
            {
                MappingMode = BrushMappingMode.Absolute,
            };
        }

        internal static LinearGradientBrush CreateParallelBrush(GradientStopCollection gradientStops, double accumLength, double totalLength, Line line)
        {
            var sp = line.StartPoint - accumLength * line.Direction;
            var ep = sp + totalLength * line.Direction;

            return new LinearGradientBrush(gradientStops, sp, ep)
            {
                MappingMode = BrushMappingMode.Absolute,
            };
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Data is null)
            {
                return base.MeasureOverride(availableSize);
            }

            this.pen.SetCurrentValue(Pen.ThicknessProperty, this.StrokeThickness);
            this.pen.SetCurrentValue(Pen.StartLineCapProperty, this.StrokeStartLineCap);
            this.pen.SetCurrentValue(Pen.EndLineCapProperty, this.StrokeEndLineCap);
            return this.Data.GetRenderBounds(this.pen).Size;
        }

        /// <inheritdoc />
        protected override void OnRender(DrawingContext dc)
        {
            if (dc is null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (this.flattenedFigures is null ||
                this.flattenedFigures.Count == 0)
            {
                return;
            }

            foreach (var figure in this.flattenedFigures)
            {
                foreach (var segment in figure.Segments)
                {
                    dc.DrawGeometry(segment.Brush, null, segment.Geometry);
                }
            }
        }

        private void OnGeometryChanged()
        {
            if (this.Data is null || this.StrokeThickness <= 0)
            {
                this.flattenedFigures = null;
                return;
            }

            var flattened = this.Data.GetFlattenedPathGeometry(this.Tolerance, ToleranceType.Absolute);
            this.flattenedFigures = flattened.Figures.Select(x => new FlattenedFigure(x, this.StrokeStartLineCap, this.StrokeEndLineCap, this.StrokeThickness))
                                                     .Where(fg => fg.Segments.Any())
                                                     .ToArray();
            this.OnGradientChanged();
        }

        private void OnGradientChanged()
        {
            if (this.flattenedFigures is null ||
                this.GradientStops is null)
            {
                return;
            }

            foreach (var figure in this.flattenedFigures)
            {
                var totalLength = this.GradientMode == GradientMode.Parallel ? figure.TotalLength : 0;
                var accumLength = 0.0;

                foreach (var segment in figure.Segments)
                {
                    var line = segment.Line;
                    segment.Brush = this.CreateBrush(line, totalLength, accumLength);
                    accumLength += line.Length;
                }
            }
        }

        private LinearGradientBrush CreateBrush(Line line, double totalLength, double accumLength)
        {
            var brush = this.GradientMode == GradientMode.Parallel
                ? CreateParallelBrush(this.GradientStops, accumLength, totalLength, line)
                : CreatePerpendicularBrush(this.GradientStops, this.StrokeThickness, line);
            brush.SetCurrentValue(GradientBrush.ColorInterpolationModeProperty, this.ColorInterpolationMode);
            return brush;
        }
    }
}
