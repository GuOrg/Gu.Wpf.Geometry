namespace Gu.Wpf.Geometry
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class GradientPath : FrameworkElement
    {
        public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        public static readonly DependencyProperty GradientStopsProperty = GradientBrush.GradientStopsProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGradientChanged));

        public static readonly DependencyProperty GradientModeProperty = DependencyProperty.Register(
            "GradientMode",
            typeof(GradientMode),
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                GradientMode.Perpendicular,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGradientChanged));

        public static readonly DependencyProperty ColorInterpolationModeProperty = GradientBrush
            .ColorInterpolationModeProperty.AddOwner(
                typeof(GradientPath),
                new FrameworkPropertyMetadata(
                    ColorInterpolationMode.SRgbLinearInterpolation,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnGradientChanged));

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                1.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        public static readonly DependencyProperty StrokeStartLineCapProperty = Shape.StrokeStartLineCapProperty.AddOwner(
                typeof(GradientPath),
                new FrameworkPropertyMetadata(
                    PenLineCap.Flat,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeEndLineCapProperty = Shape.StrokeEndLineCapProperty.AddOwner(
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                PenLineCap.Flat,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            "Tolerance",
            typeof(double),
            typeof(GradientPath),
            new FrameworkPropertyMetadata(
                Geometry.StandardFlatteningTolerance,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        private GradientGeometry gradientGeometry;

        public GradientPath()
        {
            this.GradientStops = new GradientStopCollection();
        }

        public Geometry Data
        {
            get { return (Geometry)this.GetValue(DataProperty); }
            set { this.SetValue(DataProperty, value); }
        }

        public GradientStopCollection GradientStops
        {
            get { return (GradientStopCollection)this.GetValue(GradientStopsProperty); }
            set { this.SetValue(GradientStopsProperty, value); }
        }

        public GradientMode GradientMode
        {
            get { return (GradientMode)this.GetValue(GradientModeProperty); }
            set { this.SetValue(GradientModeProperty, value); }
        }

        public ColorInterpolationMode ColorInterpolationMode
        {
            get { return (ColorInterpolationMode)this.GetValue(ColorInterpolationModeProperty); }
            set { this.SetValue(ColorInterpolationModeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)this.GetValue(StrokeThicknessProperty); }
            set { this.SetValue(StrokeThicknessProperty, value); }
        }

        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)this.GetValue(StrokeStartLineCapProperty); }
            set { this.SetValue(StrokeStartLineCapProperty, value); }
        }

        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)this.GetValue(StrokeEndLineCapProperty); }
            set { this.SetValue(StrokeEndLineCapProperty, value); }
        }

        public double Tolerance
        {
            get { return (double)this.GetValue(ToleranceProperty); }
            set { this.SetValue(ToleranceProperty, value); }
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
                MappingMode = BrushMappingMode.Absolute
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
                EndLineCap = this.StrokeEndLineCap
            };

            var rect = this.Data.GetRenderBounds(pen);
            return new Size(rect.Right, rect.Bottom);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.gradientGeometry == null)
            {
                return;
            }

            foreach (var figure in this.gradientGeometry.FigureGeometries)
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

        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GradientPath)d).OnGeometryChanged();
        }

        private static void OnGradientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GradientPath)d).OnGradientChanged();
        }

        private void OnGeometryChanged()
        {
            if (this.Data == null || this.StrokeThickness <= 0)
            {
                this.gradientGeometry = null;
                return;
            }

            this.gradientGeometry = new GradientGeometry(this.Data, this.Tolerance, this.StrokeThickness);
            this.OnGradientChanged();
        }

        private void OnGradientChanged()
        {
            if (this.gradientGeometry == null)
            {
                return;
            }

            foreach (var figure in this.gradientGeometry.FigureGeometries)
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