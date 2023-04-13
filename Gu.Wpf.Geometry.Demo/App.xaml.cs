namespace Gu.Wpf.Geometry.Demo;

using System;
using System.Windows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        if (e is { Args: { Length: 1 } args })
        {
            var window = args[0];
            this.StartupUri = new Uri($"Windows/{window}.xaml", UriKind.Relative);
        }

        base.OnStartup(e);
    }
}
