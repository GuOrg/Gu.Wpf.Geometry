namespace Gu.Wpf.Geometry.Tests
{
    using Gu.Wpf.Geometry;

    using Xunit;

    public class LineTests
    {
        [Theory]
        [InlineData("1,1; 1,2", "0,3; -1,3", "1,1; 1,3")]
        [InlineData("1,1; 1,2", "0,3; -1,3", "1,1; 1,3")]
        [InlineData("0,0; 1,0", "1,0; 2,0", "0,0; 1,0")]
        [InlineData("1,0; 2,0", "0,0; 1,0", "1,0; 2,0")]
        public void TrimOrExtendEndWith(string l1s, string l2s, string expected)
        {
            var l1 = l1s.AsLine();
            var l2 = l2s.AsLine();
            var trimmed = l1.TrimOrExtendEndWith(l2);
            Assert.Equal(expected, trimmed.ToString("F0"));
        }

        [Theory]
        [InlineData("1,1; 1,2", "0,3; -1,3", "1,3")]
        public void IntersectionPoint(string l1s, string l2s, string expected)
        {
            var l1 = l1s.AsLine();
            var l2 = l2s.AsLine();
            var actual = Line.IntersectionPoint(l1, l2);
            Assert.Equal(expected, actual.ToDebugString("F0"));
        }
    }
}
