namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;

    using NUnit.Framework;

    public static class PointAssert
    {
        public static void AreEqual(Point expected, Point actual, int digits = 2)
        {
            if (!PointComparer.Equals(expected, actual, digits))
            {
                throw new AssertionException("Points are not equal\r\n" +
                                             $"Expected: {expected.ToString("F" + digits)}\r\v" +
                                             $"Actual:   {actual.ToString("F" + digits)}");
            }
        }

        public static void AreEqual(Point? expected, Point? actual, int digits = 2)
        {
            if (expected == null)
            {
                if (actual == null)
                {
                    return;
                }

                throw new AssertionException("Points are not equal\r\n" +
                                             $"Expected: null\r\v" +
                                             $"Actual:   {actual.Value.ToString("F" + digits)}");
            }

            if (actual == null)
            {
                throw new AssertionException("Points are not equal\r\n" +
                                             $"Expected: {expected.ToString("F" + digits)}\r\v" +
                                             $"Actual:   null");
            }

            AreEqual(expected.Value, actual.Value, digits);
        }
    }
}