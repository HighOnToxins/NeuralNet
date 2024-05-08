
using BenchmarkDotNet.Running;
using System.Numerics;
using static Benchmarking.VectorBenchmarking;

namespace Benchmarking;

internal class Program
{
    public static void Main(string[] args)
    {

        var summary = BenchmarkRunner.Run<VectorBenchmarking>();
        //var summary = BenchmarkRunner.Run<StructBenchmarking>();

    }
}
