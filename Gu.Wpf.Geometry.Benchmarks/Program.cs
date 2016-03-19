namespace Gu.Wpf.Geometry.Benchmarks
{
    using BenchmarkDotNet.Running;

    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LineBenchmarks>();
        }
    }
}
