namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.UiAutomation;

    public static class ImageFolder
    {
        public static readonly string Current = GetCurrent();

        private static string GetCurrent()
        {
            if (WindowsVersion.IsWindows7() ||
                WindowsVersion.CurrentContains("Windows Server 2019"))
            {
                return "Win7";
            }

            if (WindowsVersion.IsWindows10())
            {
                return "Win10";
            }

            return WindowsVersion.CurrentVersionProductName;
        }
    }
}
