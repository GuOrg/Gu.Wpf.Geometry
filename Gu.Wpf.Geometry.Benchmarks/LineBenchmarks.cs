namespace Gu.Wpf.Geometry.Benchmarks;

using System.Windows;

using BenchmarkDotNet.Attributes;

public class LineBenchmarks
{
    private static readonly Rect Rect = new(0, 0, 10, 10);
    private static readonly Line Line = new(new Point(-1, 1), new Point(1, 1));

    [Benchmark(Baseline = true)]
    public double Division()
    {
        return 1.2345 / 2.3456;
    }

    [Benchmark]
    public double IntersectWithRectangle()
    {
        return Line.ClosestIntersection(Rect).GetValueOrDefault().X;
    }
}
