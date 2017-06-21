namespace Gu.Wpf.Geometry.UiTests.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Info
    {
        public static ProcessStartInfo ProcessStartInfo
        {
            get
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = GetExeFileName(),
                };
                return processStartInfo;
            }
        }

        internal static ProcessStartInfo CreateStartInfo(string args)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetExeFileName(),
                Arguments = args,
                UseShellExecute = false,
            };
            return processStartInfo;
        }

        private static string GetExeFileName()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var testDirectory = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
            var assemblyName = assembly.GetName().Name;
            var exeDirectoryName = assemblyName.Replace("UiTests", "Demo");
            //// ReSharper disable once PossibleNullReferenceException
            var exeDirectory = testDirectory.Replace(assemblyName, exeDirectoryName);
            var fileName = Path.Combine(exeDirectory, "Gu.Wpf.Geometry.Demo.exe");
            return fileName;
        }
    }
}
