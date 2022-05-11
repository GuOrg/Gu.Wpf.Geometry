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

        private static readonly WeakReference<UIElement?> DraggedItem = new(null);
        private static Point mousePreviousPosition;
        private static Point elementPosition;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Drag()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));

            static void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
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

            static void OnMouseLeftButtonUp(object sender, MouseEventArgs e)
            {
                if (DraggedItem.TryGetTarget(out UIElement? element))
                {
                    DraggedItem.SetTarget(null);
                    element.ReleaseMouseCapture();
                }
            }

            static void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (DraggedItem.TryGetTarget(out UIElement? element))
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

            static void OnLostMouseCapture(object sender, MouseEventArgs e)
            {
                if (DraggedItem.TryGetTarget(out _))
                {
                    DraggedItem.SetTarget(null);
                    ////element.ReleaseMouseCapture();
                }
            }
        }

        /// <summary>Helper for setting <see cref="WithMouseProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="WithMouseProperty"/> on.</param>
        /// <param name="value">WithMouse property value.</param>
        public static void SetWithMouse(this UIElement element, bool value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(WithMouseProperty, value);
        }

        /// <summary>Helper for getting <see cref="WithMouseProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="WithMouseProperty"/> from.</param>
        /// <returns>WithMouse property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetWithMouse(this UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(WithMouseProperty);
        }
    }
}
