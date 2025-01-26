using BenchmarkDotNet.Running;
using Cache.Benchmarks.CacheBenchmarks;

namespace Cache.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<LruBenchmarks>();
    }
}
