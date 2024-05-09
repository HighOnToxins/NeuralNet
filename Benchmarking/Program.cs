
using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks.VectorBenchmarks;

namespace Benchmarking;

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MatrixVectorProduct>();
    }
}
