namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using System.Drawing;
    using System.IO;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    internal static class ImageFolder
    {
        internal static readonly string Current = GetCurrent();

        internal static void AddAttachment(Exception exception, Bitmap bitmap)
        {
            if (exception is ImageAssertException { Actual: { } actual, FileName: { } fileName })
            {
                var fullFileName = Path.Combine(Path.GetTempPath(), fileName);
                _ = Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
                actual.Save(fullFileName);
                TestContext.AddTestAttachment(fullFileName);
            }
        }

        private static string GetCurrent()
        {
            if (WindowsVersion.IsWindows7())
            {
                return "Win7";
            }

            if (WindowsVersion.IsWindows10())
            {
                return "Win10";
            }

            if (WindowsVersion.CurrentContains("Windows Server 2019"))
            {
                return "WinServer2019";
            }

            return WindowsVersion.CurrentVersionProductName;
        }
    }
}
