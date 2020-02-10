namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;

    using NUnit.Framework;

    public class VectorExtTest
    {
        [TestCase("1,0", "1,0", "1,0")]
        [TestCase("2,0", "1,0", "2,0")]
        [TestCase("2,3", "1,0", "2,0")]
        [TestCase("2,-3", "1,0", "2,0")]
        [TestCase("-2,-3", "1,0", "-2,0")]
        [TestCase("-1,0", "1,0", "-1,0")]
        [TestCase("0,1", "1,0", "0,0")]
        public void ProjectOn(string v1S, string v2S, string evs)
        {
            var v1 = Vector.Parse(v1S);
            var v2 = Vector.Parse(v2S);
            var expected = Vector.Parse(evs);
            var actual = v1.ProjectOn(v2);
            VectorAssert.AreEqual(expected, actual, 2);
        }

        [TestCase("2,0", "2,0")]
        [TestCase("-2,0", "-2,0")]
        [TestCase("0,2", "0,2")]
        [TestCase("0,-2", "0,-2")]
        public void SnapToOrtho(string vs, string evs)
        {
            var v = Vector.Parse(vs);
            var expected = evs is "null" ? (Vector?)null : Vector.Parse(evs);
            var actual = v.SnapToOrtho();
            VectorAssert.AreEqual(expected, actual, 2);

            var vMinus = v.Rotate(-44);
            actual = vMinus.SnapToOrtho();
            VectorAssert.AreEqual(expected, actual, 2);

            var vPlus = v.Rotate(44);
            actual = vPlus.SnapToOrtho();
            VectorAssert.AreEqual(expected, actual, 2);
        }

        [TestCase("1,0", 90, "0,1")]
        [TestCase("2,0", 90, "0,2")]
        [TestCase("2,3", 90, "-3,2")]
        public void Rotate(string vs, double angle, string evs)
        {
            var vector = Vector.Parse(vs);
            var expected = Vector.Parse(evs);
            var actual = vector.Rotate(angle);
            VectorAssert.AreEqual(expected, actual, 2);

            var roundtrip = actual.Rotate(-angle);
            VectorAssert.AreEqual(vector, roundtrip, 2);
        }
    }
}