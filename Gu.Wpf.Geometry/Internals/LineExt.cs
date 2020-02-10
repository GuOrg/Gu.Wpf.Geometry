﻿namespace Gu.Wpf.Geometry
{
    internal static class LineExt
    {
        internal static string ToString(this Line? l, string format = "F1")
        {
            return l is null ? "null" : l.Value.ToString(format);
        }
    }
}