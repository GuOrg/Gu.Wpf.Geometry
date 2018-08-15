namespace Gu.Wpf.Geometry.Demo
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class Drag
    {
        public static readonly DependencyProperty WithMouseProperty = DependencyProperty.RegisterAttached(
            "WithMouse",
            typeof(bool),
            typeof(Drag),
            new PropertyMetadata(default(bool)));

        private static readonly WeakReference<UIElement> DraggedItem = new WeakReference<UIElement>(null);
        private static Point mousePreviousPosition;
        private static Point elementPosition;

        static Drag()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftDown));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftUp));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.LostMouseCaptureEvent, new RoutedEventHandler(OnLostMouseCapture));
        }

        public static void SetWithMouse(this UIElement element, bool value)
        {
            element.SetValue(WithMouseProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetWithMouse(this UIElement element)
        {
            return (bool)element.GetValue(WithMouseProperty);
        }

        private static void OnMouseLeftDown(object sender, MouseEventArgs e)
        {
            var element = sender as UIElement;
            if (element?.GetWithMouse() != true)
            {
                return;
            }

            DraggedItem.SetTarget(element);
            var window = Window.GetWindow(element);
            mousePreviousPosition = e.GetPosition(window);
            elementPosition = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
            _ = element.CaptureMouse();
        }

        private static void OnMouseLeftUp(object sender, MouseEventArgs e)
        {
            if (DraggedItem.TryGetTarget(out UIElement element))
            {
                DraggedItem.SetTarget(null);
                element.ReleaseMouseCapture();
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DraggedItem.TryGetTarget(out UIElement element))
            {
                var window = Window.GetWindow(element);
                var pos = e.GetPosition(window);
                var offset = pos - mousePreviousPosition;
                var newPos = elementPosition + offset;
                ////Debug.WriteLine($"Pos: {pos.ToString("F1")} mouseStart: {mousePreviousPosition.ToString("F1")} offset: {offset.ToString("F1")} newPos: {newPos.ToString("F1")}");
                element.SetCurrentValue(Canvas.LeftProperty, newPos.X);
                element.SetCurrentValue(Canvas.TopProperty, newPos.Y);
            }
        }

        private static void OnLostMouseCapture(object sender, RoutedEventArgs e)
        {
            if (DraggedItem.TryGetTarget(out UIElement _))
            {
                DraggedItem.SetTarget(null);
                ////element.ReleaseMouseCapture();
            }
        }
    }
}
