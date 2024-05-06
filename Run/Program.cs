
using BenchmarkDotNet.Running;

namespace Benchmarking;

internal class Program
{

    public static void Main(string[] args)
    {

        var summary = BenchmarkRunner.Run<StructBenchmarking>();

    }

}
