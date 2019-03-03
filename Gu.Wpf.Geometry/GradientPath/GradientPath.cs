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
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Identifies the <see cref="StrokeEndLineCap"/> dependency property.</summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty = Shape.StrokeEndLineCapProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                PenLineCap.Flat,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Identifies the <see cref="Tolerance"/> dependency property.</summary>
        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            nameof(Tolerance),
            typeof(double),
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                Geometry.StandardFlatteningTolerance,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((GradientPath)d).OnGeometryChanged()));

        private IReadOnlyList<FigureGeometry> figureGeometries;

        public GradientPath()
        {
            this.GradientStops = new GradientStopCollection();
        }

        public Geometry Data
        {
            get => (Geometry)this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        public GradientStopCollection GradientStops
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

        internal static LinearGradientBrush CreatePerpendicularBrush(
            GradientStopCollection gradientStops,
            double strokeThickness,
            Line line)
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

        internal static LinearGradientBrush CreateParallelBrush(
            GradientStopCollection gradientStops,
            double accumLength,
            double totalLength,
            Line line)
        {
            var sp = line.StartPoint - accumLength * line.Direction;
            var ep = sp + totalLength * line.Direction;

            var brush = new LinearGradientBrush(gradientStops, sp, ep)
            {
                MappingMode = BrushMappingMode.Absolute,
            };
            return brush;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Data == null)
            {
                return base.MeasureOverride(availableSize);
            }

            var pen = new Pen()
            {
                Brush = Brushes.Black,
                Thickness = this.StrokeThickness,
                StartLineCap = this.StrokeStartLineCap,
                EndLineCap = this.StrokeEndLineCap,
            };

            var rect = this.Data.GetRenderBounds(pen);
            return new Size(rect.Right, rect.Bottom);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.figureGeometries == null)
            {
                return;
            }

            foreach (var figure in this.figureGeometries)
            {
                for (var i = 0; i < figure.Lines.Count; i++)
                {
                    var patch = figure.PathGeometries[i];
                    var brush = figure.Brushes[i];
                    dc.DrawGeometry(brush, null, patch);
                }

                // Use that to draw the start line cap
                this.DrawLineCap(dc, figure.Lines.First(), this.StrokeStartLineCap, PenLineCap.Flat);
                this.DrawLineCap(dc, figure.Lines.Last(), PenLineCap.Flat, this.StrokeEndLineCap);
            }
        }

        private void OnGeometryChanged()
        {
            if (this.Data == null || this.StrokeThickness <= 0)
            {
                this.figureGeometries = null;
                return;
            }

            var flattened = this.Data.GetFlattenedPathGeometry(this.Tolerance, ToleranceType.Absolute);
            this.figureGeometries = flattened.Figures.Select(x => new FigureGeometry(x, this.StrokeThickness)).ToArray();
            this.OnGradientChanged();
        }

        private void OnGradientChanged()
        {
            if (this.figureGeometries == null)
            {
                return;
            }

            foreach (var figure in this.figureGeometries)
            {
                var totalLength = this.GradientMode == GradientMode.Parallel ? figure.TotalLength : 0;
                var accumLength = 0.0;

                for (var i = 0; i < figure.Lines.Count; i++)
                {
                    var line = figure.Lines[i];
                    figure.Brushes[i] = this.CreateBrush(line, totalLength, accumLength);
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

        private void DrawLineCap(
            DrawingContext dc,
            Line line,
            PenLineCap startLineCap,
            PenLineCap endLineCap)
        {
            if (startLineCap == PenLineCap.Flat && endLineCap == PenLineCap.Flat)
            {
                return;
            }

            var point = endLineCap == PenLineCap.Flat ? line.StartPoint : line.EndPoint;

            // Construct really tiny horizontal line
            var angle = Math.Atan2(line.Direction.Y, line.Direction.X);
            var rotate = new RotateTransform(-180 * angle / Math.PI, point.X, point.Y);
            var point1 = rotate.Transform(point);
            var point2 = rotate.Transform(point + 0.25 * line.Direction);

            // Construct pen for that line
            var pen = new Pen() { Thickness = this.StrokeThickness, StartLineCap = startLineCap, EndLineCap = endLineCap };

            // Why don't I just call dc.DrawLine at this point? Well, to avoid gaps between
            //  the tetragons, I had to draw them with an 'outlinePenWidth' pen based on the
            //  same brush as the fill. If I just called dc.DrawLine here, the caps would
            //  look a little smaller than the line, so....
            var lineGeo = new LineGeometry(point1, point2);
            var pathGeo = lineGeo.GetWidenedPathGeometry(pen);
            Brush brush;

            if (this.GradientMode == GradientMode.Perpendicular)
            {
                brush = new LinearGradientBrush(this.GradientStops, new Point(0, 0), new Point(0, 1));
                ((LinearGradientBrush)brush).SetCurrentValue(GradientBrush.ColorInterpolationModeProperty, this.ColorInterpolationMode);
            }
            else
            {
                double offset = endLineCap == PenLineCap.Flat ? 0 : 1;
                brush = new SolidColorBrush(this.GradientStops.GetColorAt(offset, this.ColorInterpolationMode));
            }

            pen = new Pen(brush, 0);
            rotate.Angle = 180 * angle / Math.PI;
            dc.PushTransform(rotate);
            dc.DrawGeometry(brush, pen, pathGeo);
            dc.Pop();
        }
    }
}
