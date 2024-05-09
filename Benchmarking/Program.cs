
using BenchmarkDotNet.Running;

namespace Benchmarking;

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkingUtil.MatrixSize = 1_000;

        BenchmarkRunner.Run<VectorBenchmarks>();
    }
}
