//using System.Collections.Generic;

//namespace Gu.Geometry.Tests
//{
//    using Gu.Wpf.Geometry;

//    public class LineComparer : IEqualityComparer<Line>
//    {
//        public static readonly LineComparer Default = new LineComparer();
//        public bool Equals(Line x, Line y)
//        {
//            return x.StartPoint.Round() == y.StartPoint.Round() && x.EndPoint.Round() == y.EndPoint.Round();
//        }

//        public int GetHashCode(Line obj)
//        {
//            unchecked
//            {
//                return (obj.StartPoint.Round().GetHashCode() * 397) ^ obj.EndPoint.Round().GetHashCode();
//            }
//        }
//    }
//}