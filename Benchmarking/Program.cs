
using BenchmarkDotNet.Running;
using Benchmarking.Tests;

namespace Benchmarking;

internal class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<FloatBenchmarks>();
        var summary1 = BenchmarkRunner.Run<StructBenchmarks>();
        var summary2 = BenchmarkRunner.Run<MatrixBenchmarks>();
        var summary3 = BenchmarkRunner.Run<VectorBenchmarks>();
    }
}
