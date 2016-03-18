namespace Gu.Wpf.Geometry
{
    using System.Windows;

    internal static class SizeExt
    {
        internal static Size AsSize(this Thickness th)
        {
            return new Size(th.Left + th.Right, th.Top + th.Bottom);
        }
    }
}