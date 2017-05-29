namespace Gu.Wpf.Geometry.UiTests.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using Gu.Wpf.Geometry.Demo;

    using NUnit.Framework;

    public static class Info
    {
        public static ProcessStartInfo ProcessStartInfo
        {
            get
            {
                var assembly = typeof(MainWindow).Assembly;
                var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
                var fileName = uri.AbsolutePath;
                var workingDirectory = Path.GetDirectoryName(fileName);
                Assert.NotNull(workingDirectory);
                var processStartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = workingDirectory,
                    FileName = fileName,
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
            var testDirestory = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
            var assemblyName = assembly.GetName().Name;
            var exeDirectoryName = assemblyName.Replace("UiTests", "Demo");
            //// ReSharper disable once PossibleNullReferenceException
            var exeDirectory = testDirestory.Replace(assemblyName, exeDirectoryName);
            var fileName = Path.Combine(exeDirectory, "Gu.Wpf.Geometry.Demo.exe");
            return fileName;
        }
    }
}
