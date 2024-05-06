
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NeuralNet;

namespace Benchmarking;

internal class Program
{

    public static void Main(string[] args)
    {

        var summary = BenchmarkRunner.Run<MatrixBenchmarks>();

    }

}
