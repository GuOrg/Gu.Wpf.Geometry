namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Media;

    public static class DependencyObjectExt
    {
        public static UIElement? GetVisualParent(this DependencyObject dependencyObject)
        {
            UIElement? element = null;
            DependencyObject reference = VisualTreeHelper.GetParent(dependencyObject);

            while (reference != null)
            {
                element = reference as UIElement;
                if (element != null)
                {
                    break;
                }

                reference = VisualTreeHelper.GetParent(reference);
            }

            return element;
        }
    }
}
