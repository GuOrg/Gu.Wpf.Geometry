namespace Gu.Wpf.Geometry.UiTests
{
    using FlaUI.Core;
    using FlaUI.Core.Definitions;
    using FlaUI.UIA3;

    using Gu.Wpf.Geometry.UiTests.Helpers;

    using NUnit.Framework;

    public class MainWindoTests
    {
        [Test]
        public void ClickAllTabs()
        {
            // Just a smoke test so that everything builds.
            using (var app = Application.Launch(Info.ProcessStartInfo))
            {
                using (var automation = new UIA3Automation())
                {
                    var window = app.GetMainWindow(automation);
                    var tab = window.FindFirstDescendant(x => x.ByControlType(ControlType.Tab));
                    foreach (var element in tab.FindAllChildren(x => x.ByControlType(ControlType.TabItem)))
                    {
                        var tabItem = element.AsTabItem();
                        tabItem.Click();
                    }
                }
            }
        }
    }
}
