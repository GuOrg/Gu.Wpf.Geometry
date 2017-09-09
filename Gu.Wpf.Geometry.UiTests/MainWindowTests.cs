namespace Gu.Wpf.Geometry.UiTests
{
    using Gu.Wpf.Geometry.UiTests.Helpers;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class MainWindowTests
    {
        [Test]
        public void ClickAllTabs()
        {
            // Just a smoke test so that we do not explode.
            using (var app = Application.Launch(Info.ProcessStartInfo))
            {
                var window = app.MainWindow;
                var tab = window.FindTabControl();
                foreach (var tabItem in tab.Items)
                {
                    tabItem.Click();
                }
            }
        }
    }
}
