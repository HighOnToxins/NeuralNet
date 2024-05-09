
using BenchmarkDotNet.Attributes;
using Benchmarking.Methods;
using Benchmarking.Objects;

namespace Benchmarking.Benchmarks.VectorBenchmarks;

public class VectorScaleBenchmarks
{
    [Benchmark]
    public StructMatrix DefaultScale() => BenchmarkingUtil.matrixA * BenchmarkingUtil.randomNum;

    [Benchmark]
    public StructMatrix InVectorScale() => MatrixScale.InVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.randomNum);

    [Benchmark]
    public StructMatrix PreVectorScale() => MatrixScale.PreVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.randomNum);

}

/* size: 100 (before fix)
| Method         | Mean         | Error       | StdDev      | Median       |
|--------------- |-------------:|------------:|------------:|-------------:|
| DefaultScale   |     8.804 us |   0.1720 us |   0.2520 us |     8.779 us |
| PreVectorScale |    63.330 us |   1.2474 us |   1.1668 us |    63.031 us |
| InVectorScale  |    54.138 us |   0.7499 us |   0.6262 us |    54.043 us |
*/

/* size: 500  (before fix)
| Method         | Mean         | Error        | StdDev       | Median       |
|--------------- |-------------:|-------------:|-------------:|-------------:|
| DefaultScale   |     323.2 us |      5.42 us |      5.07 us |     323.3 us |
| PreVectorScale |   3,853.9 us |     76.78 us |    147.93 us |   3,887.7 us |
| InVectorScale  |   2,201.2 us |     56.84 us |    165.81 us |   2,153.4 us |
*/



/* size: 1000  (before fix)
| Method         | Mean         | Error      | StdDev      |
|--------------- |-------------:|-----------:|------------:|
| DefaultAdd     |     2.326 ms |  0.0336 ms |   0.0314 ms |
| PreVectorAdd   |    13.226 ms |  0.2627 ms |   0.3416 ms |
| InVectorAdd    |     9.502 ms |  0.1874 ms |   0.1753 ms |


| DefaultScale   |     1.456 ms |  0.0238 ms |   0.0211 ms |
| PreVectorScale |    11.304 ms |  0.1913 ms |   0.2046 ms |
| InVectorScale  |     7.118 ms |  0.0517 ms |   0.0403 ms |


| DefaultProd    | 1,311.372 ms | 18.5551 ms |  17.3565 ms |
| PreVectorProd  |   221.525 ms |  4.2910 ms |   4.7694 ms |
| InVectorProd   | 4,907.852 ms | 90.7938 ms | 100.9171 ms |
*/

/* size 100 (after fix)
| Method         | Mean         | Error      | StdDev     |
|--------------- |-------------:|-----------:|-----------:|
| DefaultScale   |     8.594 us |  0.1205 us |  0.1006 us |
| PreVectorScale |    63.814 us |  0.6000 us |  0.5010 us |
| InVectorScale  |    56.068 us |  1.1016 us |  1.5443 us |
*/
