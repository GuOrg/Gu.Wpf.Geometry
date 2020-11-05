namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// An elliptical balloon.
    /// </summary>
    public class EllipseBalloon : BalloonBase
    {
        private static readonly DependencyProperty EllipseProperty = DependencyProperty.Register(
            "Ellipse",
            typeof(Ellipse),
            typeof(EllipseBalloon),
            new PropertyMetadata(default(Ellipse)));

        /// <inheritdoc/>
        protected override Geometry GetOrCreateBoxGeometry(Size renderSize)
        {
            var ellipse = Ellipse.CreateFromSize(renderSize);
            this.SetCurrentValue(EllipseProperty, ellipse);
            if (ellipse.RadiusX <= 0 || ellipse.RadiusY <= 0)
            {
                return Geometry.Empty;
            }

            if (this.BoxGeometry is EllipseGeometry)
            {
                return this.BoxGeometry;
            }

            var geometry = new EllipseGeometry();
            _ = geometry.Bind(EllipseGeometry.CenterProperty)
                        .OneWayTo(this, EllipseProperty, EllipseCenterConverter.Default);
            _ = geometry.Bind(EllipseGeometry.RadiusXProperty)
                        .OneWayTo(this, EllipseProperty, EllipseRadiusXConverter.Default);
            _ = geometry.Bind(EllipseGeometry.RadiusYProperty)
                        .OneWayTo(this, EllipseProperty, EllipseRadiusYConverter.Default);
            return geometry;
        }

        /// <inheritdoc/>
        protected override Geometry GetOrCreateConnectorGeometry(Size renderSize)
        {
            var ellipse = Ellipse.CreateFromSize(renderSize);
            this.SetCurrentValue(EllipseProperty, ellipse);
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

            this.SetCurrentValue(ConnectorVertexPointProperty, vertexPoint);
            this.SetCurrentValue(ConnectorPoint1Property, p1);
            this.SetCurrentValue(ConnectorPoint2Property, p2);
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

        /// <inheritdoc/>
        protected override void UpdateConnectorOffset()
        {
            var hasTarget =
                this.PlacementTarget?.IsVisible == true ||
                !this.PlacementRectangle.IsEmpty;

            if (this.IsVisible && this.RenderSize.Width > 0 && hasTarget)
            {
                if (!this.IsLoaded)
                {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                    _ = this.Dispatcher.BeginInvoke(new Action(this.UpdateConnectorOffset), DispatcherPriority.Loaded);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
                    return;
                }

                var selfRect = new Rect(new Point(0, 0).ToScreen(this), this.RenderSize).ToScreen(this);
                var targetRect = this.GetTargetRect();
                var ellipse = new Ellipse(selfRect);

                var tp = this.PlacementOptions?.GetPointOnTarget(selfRect, targetRect);
                if (tp is null || ellipse.Contains(tp.Value))
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
                //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (ip is null)
                {
                    // failing silently in release
                    this.InvalidateProperty(ConnectorOffsetProperty);
                }
                else
                {
                    var v = tp.Value - ip.Value;

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (this.PlacementOptions != null && v.Length > 0 && this.PlacementOptions.Offset != 0)
                    {
                        v -= this.PlacementOptions.Offset * v.Normalized();
                    }

                    this.SetCurrentValue(ConnectorOffsetProperty, v);
                }
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        private PathFigure CreatePathFigureStartingAt(DependencyProperty property)
        {
            var figure = new PathFigure { IsClosed = true };
            _ = figure.Bind(PathFigure.StartPointProperty)
                      .OneWayTo(this, property);
            return figure;
        }

        private LineSegment CreateLineSegmentTo(DependencyProperty property)
        {
            var lineSegment = new LineSegment { IsStroked = true };
            _ = lineSegment.Bind(LineSegment.PointProperty)
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
                //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (toEllipseCenter is null)
                {
                    // this should never happen but failing silently
                    // the balloons should not throw much returning random point.
                    return ellipse.CenterPoint;
                }

                return ellipse.PointOnCircumference(toEllipseCenter.Value.Direction.Negated());
            }
        }

        private sealed class EllipseCenterConverter : IValueConverter
        {
            internal static readonly EllipseCenterConverter Default = new EllipseCenterConverter();

            private EllipseCenterConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                // ReSharper disable once PossibleNullReferenceException
                return ((Ellipse)value).CenterPoint;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class EllipseRadiusXConverter : IValueConverter
        {
            internal static readonly EllipseRadiusXConverter Default = new EllipseRadiusXConverter();

            private EllipseRadiusXConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                // ReSharper disable once PossibleNullReferenceException
                return ((Ellipse)value).RadiusX;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class EllipseRadiusYConverter : IValueConverter
        {
            internal static readonly EllipseRadiusYConverter Default = new EllipseRadiusYConverter();

            private EllipseRadiusYConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                // ReSharper disable once PossibleNullReferenceException
                return ((Ellipse)value).RadiusY;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }
    }
}
