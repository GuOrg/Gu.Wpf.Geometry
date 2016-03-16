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
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        public static readonly DependencyProperty GradientStopsProperty = GradientBrush.GradientStopsProperty.AddOwner(
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGradientChanged));

        public static readonly DependencyProperty GradientModeProperty = DependencyProperty.Register(
            "GradientMode",
            typeof (GradientMode),
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                GradientMode.Perpendicular,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGradientChanged));

        public static readonly DependencyProperty ColorInterpolationModeProperty = GradientBrush
            .ColorInterpolationModeProperty.AddOwner(
                typeof (GradientPath),
                new FrameworkPropertyMetadata(
                    ColorInterpolationMode.SRgbLinearInterpolation,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnGradientChanged));

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                1.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        public static readonly DependencyProperty StrokeStartLineCapProperty = Shape.StrokeStartLineCapProperty.AddOwner
            (
                typeof (GradientPath),
                new FrameworkPropertyMetadata(
                    PenLineCap.Flat,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeEndLineCapProperty = Shape.StrokeEndLineCapProperty.AddOwner(
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                PenLineCap.Flat,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            "Tolerance",
            typeof (double),
            typeof (GradientPath),
            new FrameworkPropertyMetadata(
                System.Windows.Media.Geometry.StandardFlatteningTolerance,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGeometryChanged));

        private GradientGeometry _gradientGeometry;

        public GradientPath()
        {
            GradientStops = new GradientStopCollection();
        }

        public System.Windows.Media.Geometry Data
        {
            set { SetValue(DataProperty, value); }
            get { return (System.Windows.Media.Geometry) GetValue(DataProperty); }
        }

        public GradientStopCollection GradientStops
        {
            set { SetValue(GradientStopsProperty, value); }
            get { return (GradientStopCollection) GetValue(GradientStopsProperty); }
        }

        public GradientMode GradientMode
        {
            set { SetValue(GradientModeProperty, value); }
            get { return (GradientMode) GetValue(GradientModeProperty); }
        }

        public ColorInterpolationMode ColorInterpolationMode
        {
            set { SetValue(ColorInterpolationModeProperty, value); }
            get { return (ColorInterpolationMode) GetValue(ColorInterpolationModeProperty); }
        }

        public double StrokeThickness
        {
            set { SetValue(StrokeThicknessProperty, value); }
            get { return (double) GetValue(StrokeThicknessProperty); }
        }

        public PenLineCap StrokeStartLineCap
        {
            set { SetValue(StrokeStartLineCapProperty, value); }
            get { return (PenLineCap) GetValue(StrokeStartLineCapProperty); }
        }

        public PenLineCap StrokeEndLineCap
        {
            set { SetValue(StrokeEndLineCapProperty, value); }
            get { return (PenLineCap) GetValue(StrokeEndLineCapProperty); }
        }

        public double Tolerance
        {
            set { SetValue(ToleranceProperty, value); }
            get { return (double) GetValue(ToleranceProperty); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Data == null)
            {
                return base.MeasureOverride(availableSize);
            }

            var pen = new Pen()
            {
                Brush = Brushes.Black,
                Thickness = StrokeThickness,
                StartLineCap = StrokeStartLineCap,
                EndLineCap = StrokeEndLineCap
            };

            var rect = Data.GetRenderBounds(pen);
            return new Size(rect.Right, rect.Bottom);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_gradientGeometry == null)
            {
                return;
            }

            foreach (var figure in _gradientGeometry.FigureGeometries)
            {
                for (var i = 0; i < figure.Lines.Count; i++)
                {
                    var patch = figure.PathGeometries[i];
                    var brush = figure.Brushes[i];
                    dc.DrawGeometry(brush, null, patch);
                }

                // Use that to draw the start line cap
                DrawLineCap(dc, figure.Lines.First(), StrokeStartLineCap, PenLineCap.Flat);
                DrawLineCap(dc, figure.Lines.Last(), PenLineCap.Flat, StrokeEndLineCap);
            }
        }

        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GradientPath) d).OnGeometryChanged();
        }

        private static void OnGradientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GradientPath) d).OnGradientChanged();
        }

        private void OnGeometryChanged()
        {
            if (Data == null || StrokeThickness <= 0)
            {
                _gradientGeometry = null;
                return;
            }
            _gradientGeometry = new GradientGeometry(Data, Tolerance, StrokeThickness);
            OnGradientChanged();
        }

        private void OnGradientChanged()
        {
            if (_gradientGeometry == null)
            {
                return;
            }
            foreach (var figure in _gradientGeometry.FigureGeometries)
            {
                var totalLength = GradientMode == GradientMode.Parallel ? figure.TotalLength : 0;
                var accumLength = 0.0;

                for (var i = 0; i < figure.Lines.Count; i++)
                {
                    var line = figure.Lines[i];
                    figure.Brushes[i] = CreateBrush(line, totalLength, accumLength);
                    accumLength += line.Length;
                }
            }
        }

        private LinearGradientBrush CreateBrush(Line line, double totalLength, double accumLength)
        {
            LinearGradientBrush brush;
            if (GradientMode == GradientMode.Parallel)
            {
                brush = CreateParallelBrush(GradientStops, accumLength, totalLength, line);
            }
            else
            {
                brush = CreatePerpendicularBrush(GradientStops, StrokeThickness, line);
            }
            brush.ColorInterpolationMode = ColorInterpolationMode;
            return brush;
        }

        internal static LinearGradientBrush CreatePerpendicularBrush(
            GradientStopCollection gradientStops,
            double strokeThickness,
            Line line)
        {
            var mp = line.MidPoint;
            var v = line.PerpendicularDirection;
            var startPoint = mp.Offset(v, strokeThickness/2);
            var endPoint = mp.Offset(v, -strokeThickness/2);
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
            var sp = line.StartPoint - accumLength*line.Direction;
            var ep = sp + totalLength*line.Direction;

            var brush = new LinearGradientBrush(gradientStops, sp, ep)
            {
                MappingMode = BrushMappingMode.Absolute
            };
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
            var rotate = new RotateTransform(-180*angle/Math.PI, point.X, point.Y);
            var point1 = rotate.Transform(point);
            var point2 = rotate.Transform(point + 0.25*line.Direction);

            // Construct pen for that line
            var pen = new Pen() {Thickness = StrokeThickness, StartLineCap = startLineCap, EndLineCap = endLineCap};

            // Why don't I just call dc.DrawLine at this point? Well, to avoid gaps between 
            //  the tetragons, I had to draw them with an 'outlinePenWidth' pen based on the 
            //  same brush as the fill. If I just called dc.DrawLine here, the caps would 
            //  look a little smaller than the line, so....

            var lineGeo = new LineGeometry(point1, point2);
            var pathGeo = lineGeo.GetWidenedPathGeometry(pen);
            Brush brush = null;

            if (GradientMode == GradientMode.Perpendicular)
            {
                brush = new LinearGradientBrush(GradientStops, new Point(0, 0), new Point(0, 1));
                (brush as LinearGradientBrush).ColorInterpolationMode = ColorInterpolationMode;
            }
            else
            {
                double offset = endLineCap == PenLineCap.Flat ? 0 : 1;
                brush = new SolidColorBrush(GradientStops.GetColorAt(offset, ColorInterpolationMode));
            }

            pen = new Pen(brush, 0);
            rotate.Angle = 180*angle/Math.PI;
            dc.PushTransform(rotate);
            dc.DrawGeometry(brush, pen, pathGeo);
            dc.Pop();
        }
    }
}