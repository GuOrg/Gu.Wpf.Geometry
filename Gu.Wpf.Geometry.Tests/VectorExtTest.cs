namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;
    using Xunit;

    public class VectorExtTest
    {
        [Theory]
        [InlineData("1,0", "1,0", "1,0")]
        [InlineData("2,0", "1,0", "2,0")]
        [InlineData("2,3", "1,0", "2,0")]
        [InlineData("2,-3", "1,0", "2,0")]
        [InlineData("-2,-3", "1,0", "-2,0")]
        [InlineData("-1,0", "1,0", "-1,0")]
        [InlineData("0,1", "1,0", "0,0")]
        public void ProjectOn(string v1s, string v2s, string evs)
        {
            var v1 = Vector.Parse(v1s);
            var v2 = Vector.Parse(v2s);
            var expected = Vector.Parse(evs);
            var actual = v1.ProjectOn(v2);
            Assert.Equal(expected, actual, VectorComparer.Default);
        }

        [Theory]
        [InlineData("2,0", "2,0")]
        [InlineData("-2,0", "-2,0")]
        [InlineData("0,2", "0,2")]
        [InlineData("0,-2", "0,-2")]
        public void SnapToOrtho(string vs, string evs)
        {
            var v = Vector.Parse(vs);
            var expected = evs == "null" ? (Vector?)null : Vector.Parse(evs);
            var actual = v.SnapToOrtho();
            Assert.Equal(expected, actual, NullableVectorComparer.Default);

            var vMinus = v.Rotate(-44);
            actual = vMinus.SnapToOrtho();
            Assert.Equal(expected, actual, NullableVectorComparer.Default);

            var vPlus = v.Rotate(44);
            actual = vPlus.SnapToOrtho();
            Assert.Equal(expected, actual, NullableVectorComparer.Default);
        }
    }
}