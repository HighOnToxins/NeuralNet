
using BenchmarkDotNet.Attributes;
using Benchmarking.Methods;
using Benchmarking.Objects;

namespace Benchmarking.Benchmarks.VectorBenchmarks;

public class VectorProdBenchmarks
{
    [Benchmark]
    public StructMatrix DefaultProd() => BenchmarkingUtil.matrixA * BenchmarkingUtil.matrixB;

    [Benchmark]
    public StructMatrix PreVectorProd() => MatrixProduct.PreVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

    [Benchmark]
    public StructMatrix InVectorProd() => MatrixProduct.InVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

    [Benchmark]
    public StructMatrix VectorMatrixProd() => MatrixProduct.UsingVectorMatrix(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

    [Benchmark]
    public StructMatrix OptimizedPreProd() => MatrixProduct.OptimizedPreVector(BenchmarkingUtil.matrixA, BenchmarkingUtil.matrixB);

}

/* size: 100 (before fix)
| Method         | Mean         | Error       | StdDev      | Median       |
|--------------- |-------------:|------------:|------------:|-------------:|
| DefaultProd    | 1,377.847 us |  19.3555 us |  16.1627 us | 1,370.131 us |
| PreVectorProd  |   308.547 us |   6.1158 us |   7.7345 us |   305.625 us |
| InVectorProd   | 5,566.406 us | 155.9426 us | 449.9302 us | 5,323.045 us |
*/

/* size: 500  (before fix)
| Method         | Mean         | Error        | StdDev       | Median       |
|--------------- |-------------:|-------------:|-------------:|-------------:|
| DefaultProd    | 165,112.4 us |  2,205.06 us |  1,954.72 us | 165,562.8 us |
| PreVectorProd  |  40,841.6 us |  2,334.49 us |  6,883.30 us |  42,256.9 us |
| InVectorProd   | 617,909.0 us | 10,883.53 us | 13,365.95 us | 616,641.8 us |
*/


/* size: 1000  (before fix)
| Method         | Mean         | Error      | StdDev      |
|--------------- |-------------:|-----------:|------------:|
| DefaultProd    | 1,311.372 ms | 18.5551 ms |  17.3565 ms |
| PreVectorProd  |   221.525 ms |  4.2910 ms |   4.7694 ms |
| InVectorProd   | 4,907.852 ms | 90.7938 ms | 100.9171 ms |
*/

/* size 100 (after fix)
| Method         | Mean         | Error      | StdDev     |
|--------------- |-------------:|-----------:|-----------:|
| DefaultProd    | 1,630.031 us | 23.8919 us | 21.1795 us |
| PreVectorProd  |   302.989 us |  5.7470 us |  8.7763 us |
| InVectorProd   | 5,256.522 us | 82.2249 us | 72.8902 us |
*/


/* size 100
| Method           | Mean       | Error    | StdDev   |
|----------------- |-----------:|---------:|---------:|
| DefaultProd      | 1,652.0 us | 25.45 us | 21.25 us |
| PreVectorProd    |   307.6 us |  6.14 us | 12.82 us |
| VectorMatrixProd |   371.4 us |  7.39 us | 11.94 us |
*/
