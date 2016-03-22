namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class EllipseBalloon : BalloonBase
    {
        private static readonly DependencyProperty EllipseProperty = DependencyProperty.Register(
            "Ellipse",
            typeof(Ellipse),
            typeof(EllipseBalloon),
            new PropertyMetadata(default(Ellipse)));

        protected override Geometry GetOrCreateBoxGeometry(Size renderSize)
        {
            var ellipse = Ellipse.CreateFromSize(renderSize);
            this.SetValue(EllipseProperty, ellipse);
            if (ellipse.RadiusX <= 0 || ellipse.RadiusY <= 0)
            {
                return Geometry.Empty;
            }

            if (this.BoxGeometry is EllipseGeometry)
            {
                return this.BoxGeometry;
            }

            var geometry = new EllipseGeometry();
            geometry.Bind(EllipseGeometry.CenterProperty)
                    .OneWayTo(this, EllipseProperty, EllipseCenterConverter.Default);
            geometry.Bind(EllipseGeometry.RadiusXProperty)
                    .OneWayTo(this, EllipseProperty, EllipseRadiusXConverter.Default);
            geometry.Bind(EllipseGeometry.RadiusYProperty)
                    .OneWayTo(this, EllipseProperty, EllipseRadiusYConverter.Default);
            return geometry;
        }

        protected override Geometry GetOrCreateConnectorGeometry(Size renderSize)
        {
            var ellipse = Ellipse.CreateFromSize(renderSize);
            this.SetValue(EllipseProperty, ellipse);
            if (ellipse.IsZero)
            {
                return Geometry.Empty;
            }

            var direction = this.ConnectorOffset;
            var ip = ellipse.PointOnCircumference(direction);
            var vertexPoint = ip + this.ConnectorOffset;
            var ray = new Ray(vertexPoint, this.ConnectorOffset.Negated());

            var p1 = ConnectorPoint.Find(ray, this.ConnectorAngle / 2, this.StrokeThickness, ellipse);
            var p2 = ConnectorPoint.Find(ray, -this.ConnectorAngle / 2, this.StrokeThickness, ellipse);

            this.SetValue(ConnectorVertexPointProperty, vertexPoint);
            this.SetValue(ConnectorPoint1Property, p1);
            this.SetValue(ConnectorPoint2Property, p2);
            if (this.ConnectorGeometry is PathGeometry)
            {
                return this.ConnectorGeometry;
            }

            var figure = this.CreatePathFigureStartingAt(ConnectorPoint1Property);
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorVertexPointProperty));
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorPoint2Property));
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        protected override void UpdateConnectorOffset()
        {
            if (this.IsVisible && this.RenderSize.Width > 0 && this.PlacementTarget?.IsVisible == true)
            {
                if (!this.IsLoaded)
                {
                    this.Dispatcher.Invoke(this.UpdateConnectorOffset, DispatcherPriority.Loaded);
                    return;
                }

                var selfRect = new Rect(new Point(0, 0).ToScreen(this), this.RenderSize).ToScreen(this);
                var ellipse = new Ellipse(selfRect);
                var targetRect = new Rect(new Point(0, 0).ToScreen(this.PlacementTarget), this.PlacementTarget.RenderSize).ToScreen(this);
                var tp = this.PlacementOptions?.GetPointOnTarget(selfRect, targetRect);
                if (tp == null || ellipse.Contains(tp.Value))
                {
                    this.InvalidateProperty(ConnectorOffsetProperty);
                    return;
                }

                if (ellipse.Contains(tp.Value))
                {
                    this.SetCurrentValue(ConnectorOffsetProperty, new Vector(0, 0));
                    return;
                }

                var mp = ellipse.CenterPoint;
                var ip = new Ray(mp, mp.VectorTo(tp.Value)).FirstIntersectionWith(ellipse);
                Debug.Assert(ip != null, "Did not find an intersection, bug in the library");
                if (ip == null)
                {
                    // failing silently in release
                    this.InvalidateProperty(ConnectorOffsetProperty);
                }

                var v = tp.Value - ip.Value;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (this.PlacementOptions != null && v.Length > 0 && this.PlacementOptions.Offset != 0)
                {
                    v = v - this.PlacementOptions.Offset * v.Normalized();
                }

                this.SetCurrentValue(ConnectorOffsetProperty, v);
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        private PathFigure CreatePathFigureStartingAt(DependencyProperty property)
        {
            var figure = new PathFigure { IsClosed = true };
            figure.Bind(PathFigure.StartPointProperty)
                .OneWayTo(this, property);
            return figure;
        }

        private LineSegment CreateLineSegmentTo(DependencyProperty property)
        {
            var lineSegment = new LineSegment { IsStroked = true };
            lineSegment.Bind(LineSegment.PointProperty)
                .OneWayTo(this, property);
            return lineSegment;
        }

        private static class ConnectorPoint
        {
            internal static Point Find(Ray ray, double angle, double strokeThickness, Ellipse ellipse)
            {
                return Find(ray.Rotate(angle), strokeThickness, ellipse);
            }

            private static Point Find(Ray ray, double strokeThickness, Ellipse ellipse)
            {
                var ip = ray.FirstIntersectionWith(ellipse);
                if (ip != null)
                {
                    return ip.Value + strokeThickness * ray.Direction;
                }

                return FindTangentPoint(ray, ellipse);
            }

            private static Point FindTangentPoint(Ray toCenter, Ellipse ellipse)
            {
                var toEllipseCenter = toCenter.PerpendicularLineTo(ellipse.CenterPoint);
                Debug.Assert(toEllipseCenter != null, "Ray should not go through ellipse center here");
                if (toEllipseCenter == null)
                {
                    // this should never happen but failing silently
                    // the balloons should not throw much returning random point.
                    return ellipse.CenterPoint;
                }

                return ellipse.PointOnCircumference(toEllipseCenter.Value.Direction.Negated());
            }
        }

        private class EllipseCenterConverter : IValueConverter
        {
            internal static readonly EllipseCenterConverter Default = new EllipseCenterConverter();

            private EllipseCenterConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).CenterPoint;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private class EllipseRadiusXConverter : IValueConverter
        {
            internal static readonly EllipseRadiusXConverter Default = new EllipseRadiusXConverter();

            private EllipseRadiusXConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).RadiusX;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private class EllipseRadiusYConverter : IValueConverter
        {
            internal static readonly EllipseRadiusYConverter Default = new EllipseRadiusYConverter();

            private EllipseRadiusYConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).RadiusY;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }
    }
}