namespace Gu.Wpf.Geometry.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class MainWindowTests
    {
        [Test]
        public void ClickAllTabs()
        {
            // Just a smoke test so that we do not explode.
            using var app = Application.Launch("Gu.Wpf.Geometry.Demo.exe");
            app.WaitForMainWindow(TimeSpan.FromSeconds(10));
            var window = app.MainWindow;
            var tab = window.FindTabControl();
            foreach (var tabItem in tab.Items)
            {
                _ = tabItem.Select();
            }
        }
    }
}
