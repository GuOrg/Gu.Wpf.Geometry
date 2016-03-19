namespace Gu.Wpf.Geometry
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;

    internal static class VisualTreeHelperExt
    {
        internal static IEnumerable<DependencyObject> Ancestors(this DependencyObject o)
        {
            var parent = VisualTreeHelper.GetParent(o);
            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        internal static T VisualChild<T>(this Visual parent)
            where T : Visual
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 1)
            {
                throw new InvalidOperationException("Expected single child");
            }

            if (count == 0)
            {
                return default(T);
            }

            return (T)VisualTreeHelper.GetChild(parent, 0);
        }

        internal static IEnumerable<Visual> VisualChildren(this Visual parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                yield return (Visual)VisualTreeHelper.GetChild(parent, i);
            }
        }

        internal static IEnumerable<DependencyObject> NestedChildren(this DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                yield return child;
                if (VisualTreeHelper.GetChildrenCount(child) != 0)
                {
                    foreach (var nestedChild in NestedChildren(child))
                    {
                        yield return nestedChild;
                    }
                }
            }
        }

        internal static T SingleOrNull<T>(this IEnumerable<object> items)
            where T : class
        {
            T match = null;
            foreach (var item in items)
            {
                var temp = item as T;
                if (temp != null)
                {
                    if (match != null)
                    {
                        return null;
                    }

                    match = temp;
                }
            }

            return match;
        }

        internal static string DumpVisualTree(this DependencyObject parent)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    DumpVisualTree(parent, writer);
                    return stringWriter.ToString();
                }
            }
        }

        private static void DumpVisualTree(this DependencyObject parent, IndentedTextWriter writer)
        {
            writer.WriteLine(parent.GetType().Name);
            if (VisualTreeHelper.GetChildrenCount(parent) != 0)
            {
                writer.Indent++;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    DumpVisualTree(child, writer);
                }

                writer.Indent--;
            }
        }
    }
}