namespace Gu.Wpf.Geometry.UiTests
{
    using System;

    public static class Env
    {
        public static bool IsAppVeyor => Environment.GetEnvironmentVariable("APPVEYOR") != null;
    }
}
