using BenchmarkDotNet.Attributes;
using Benchmarking.Methods;
using Benchmarking.Objects;

namespace Benchmarking.Benchmarks.VectorBenchmarks;

public class VectorAddBenchmarks
{
    [Benchmark]
    public StructMatrix DefaultAdd() => BenchmarkingUtil.matrixA + BenchmarkingUtil.matrixB;

    [Benchmark]
    public StructMatrix InVectorAdd() => MatrixAdd.InVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

    [Benchmark]
    public StructMatrix PreVectorAdd() => MatrixAdd.PreVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

}

/* size: 100 (before fix)
| Method         | Mean         | Error       | StdDev      | Median       |
|--------------- |-------------:|------------:|------------:|-------------:|
| DefaultAdd     |    18.546 us |   0.3669 us |   0.6231 us |    18.481 us |
| PreVectorAdd   |    66.016 us |   1.2697 us |   1.3586 us |    65.636 us |
| InVectorAdd    |    80.565 us |   1.5524 us |   1.3762 us |    81.029 us |
*/

/* size: 500  (before fix)
| Method         | Mean         | Error        | StdDev       | Median       |
|--------------- |-------------:|-------------:|-------------:|-------------:|
| DefaultAdd     |     552.1 us |     10.28 us |      9.62 us |     551.5 us |
| PreVectorAdd   |   3,765.6 us |     90.48 us |    266.78 us |   3,752.8 us |
| InVectorAdd    |   3,189.7 us |    220.59 us |    650.41 us |   3,053.7 us |
*/


/* size: 1000  (before fix)
| Method         | Mean         | Error      | StdDev      |
|--------------- |-------------:|-----------:|------------:|
| DefaultAdd     |     2.326 ms |  0.0336 ms |   0.0314 ms |
| PreVectorAdd   |    13.226 ms |  0.2627 ms |   0.3416 ms |
| InVectorAdd    |     9.502 ms |  0.1874 ms |   0.1753 ms |
*/

/* size 100 (after fix)
| Method         | Mean         | Error      | StdDev     |
|--------------- |-------------:|-----------:|-----------:|
| DefaultAdd     |    12.917 us |  0.2220 us |  0.3522 us |
| PreVectorAdd   |    66.148 us |  0.8088 us |  0.6754 us |
| InVectorAdd    |    80.488 us |  1.5358 us |  1.8283 us |
*/
