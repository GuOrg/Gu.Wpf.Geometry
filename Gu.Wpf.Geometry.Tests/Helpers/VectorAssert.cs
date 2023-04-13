namespace Gu.Wpf.Geometry.Tests;

using System.Windows;

using NUnit.Framework;

public static class VectorAssert
{
    public static void AreEqual(Vector expected, Vector actual, int digits = 2)
    {
        if (!VectorComparer.Equals(expected, actual, digits))
        {
            throw new AssertionException("Vectors are not equal\r\n" +
                                         $"Expected: {expected.ToString("F" + digits)}\r\v" +
                                         $"Actual:   {actual.ToString("F" + digits)}");
        }
    }

    public static void AreEqual(Vector? expected, Vector? actual, int digits = 2)
    {
        if (expected is null)
        {
            if (actual is null)
            {
                return;
            }

            throw new AssertionException("Vectors are not equal\r\n" +
                                         $"Expected: null\r\v" +
                                         $"Actual:   {actual.Value.ToString("F" + digits)}");
        }

        if (actual is null)
        {
            throw new AssertionException("Vectors are not equal\r\n" +
                                         $"Expected: {expected.ToString("F" + digits)}\r\v" +
                                         $"Actual:   null");
        }

        AreEqual(expected.Value, actual.Value, digits);
    }
}
