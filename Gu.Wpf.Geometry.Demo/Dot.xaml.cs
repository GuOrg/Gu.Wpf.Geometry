﻿namespace Gu.Wpf.Geometry.Demo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class Dot : UserControl
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            nameof(Center),
            typeof(Point),
            typeof(Dot),
            new PropertyMetadata(default(Point), OnCenterChanged));

        private bool isDragging;
        private Point mouseDragStart;
        private Point dragStartPos;

        public Dot()
        {
            this.InitializeComponent();
        }

        public Point Center
        {
            get => (Point)this.GetValue(CenterProperty);
            set => this.SetValue(CenterProperty, value);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonDown(args);
            this.isDragging = true;
            this.mouseDragStart = args.GetPosition(this);
            this.dragStartPos = this.Center;
            this.CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (!this.isDragging)
            {
                return;
            }

            var pos = args.GetPosition(this);
            var offset = pos - this.mouseDragStart;
            var center = this.dragStartPos + offset;
            this.SetCurrentValue(CenterProperty, center);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);
            if (!this.isDragging)
            {
                return;
            }

            this.isDragging = false;
            this.ReleaseMouseCapture();
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            this.isDragging = false;
            base.OnLostMouseCapture(args);
        }

        private static void OnCenterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Dot)obj).EllipseGeometry.SetCurrentValue(System.Windows.Media.EllipseGeometry.CenterProperty, (Point)args.NewValue);
        }
    }
}
