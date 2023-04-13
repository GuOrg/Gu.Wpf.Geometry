namespace Gu.Wpf.Geometry.UiTests;

using System.Drawing;
using System.IO;

using Gu.Wpf.UiAutomation;

using NUnit.Framework;

public static class TestImage
{
    internal static readonly string Current = GetCurrent();

    public static void AreEqual(string directory, string fileName, UiElement element)
    {
        var fullFileName = $"Images\\{directory}\\{TestImage.Current}\\{fileName}";
        if (File.Exists(fullFileName))
        {
            ImageAssert.AreEqual(fullFileName, element, OnFail);
        }
        else
        {
            ImageAssert.AreEqual($"Images\\{directory}\\{fileName}", element, OnFail);
        }
    }

    [Explicit]
    [Script]
    public static void Purge()
    {
        foreach (var dir in Directory.EnumerateDirectories(@"C:\Git\_GuOrg\Gu.Wpf.Geometry\Gu.Wpf.Geometry.UiTests\Images", "*.*", SearchOption.TopDirectoryOnly))
        {
            foreach (var file in Directory.EnumerateFiles(dir, "*.png", SearchOption.TopDirectoryOnly))
            {
                using var image = (Bitmap)Image.FromFile(file);
                foreach (var candidate in Directory.EnumerateFiles(dir, Path.GetFileName(file), SearchOption.AllDirectories))
                {
                    if (candidate != file)
                    {
                        using var candidateImage = (Bitmap)Image.FromFile(candidate);
                        if (ImageAssert.Equal(image, candidateImage))
                        {
                            File.Delete(candidate);
                        }
                    }
                }
            }
        }
    }

#pragma warning disable IDE0060, CA1801 // Remove unused parameter
    internal static void OnFail(Bitmap? expected, Bitmap actual, string resource)
#pragma warning restore IDE0060, CA1801  // Remove unused parameter
    {
        var fullFileName = Path.Combine(Path.GetTempPath(), resource);
        _ = Directory.CreateDirectory(Path.GetDirectoryName(fullFileName)!);
        actual.Save(fullFileName);
        TestContext.AddTestAttachment(fullFileName);
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

        if (WindowsVersion.CurrentContains("Windows Server 2022"))
        {
            return "WinServer2022";
        }

        return WindowsVersion.CurrentVersionProductName;
    }
}
