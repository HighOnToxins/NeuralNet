
using BenchmarkDotNet.Attributes;
using Benchmarking.Methods;
using Benchmarking.Objects;

namespace Benchmarking.Benchmarks.VectorBenchmarks;

public class MatrixVectorProduct
{

    [Benchmark]
    public StructVector DefaultVectorMatrixProd() => BenchmarkingUtil.matrixA * BenchmarkingUtil.vectorA;

    [Benchmark]
    public StructVector VectorVectorMatrixProd() => VectorMatrixProduct.VectorProd(BenchmarkingUtil.matrixA, BenchmarkingUtil.vectorA);

    [Benchmark]
    public StructVector VectorVectorMatrixProdOptimized() => VectorMatrixProduct.VectorProdOptimized(BenchmarkingUtil.matrixA, BenchmarkingUtil.vectorA);

    [Benchmark]
    public StructVector VectorVectorMatrixProdOptimized2() => VectorMatrixProduct.VectorProdOptimized2(BenchmarkingUtil.matrixA, BenchmarkingUtil.vectorA);
}

/* size 1000
| Method                           | Mean     | Error     | StdDev    |
|--------------------------------- |---------:|----------:|----------:|
| DefaultVectorMatrixProd          | 1.052 ms | 0.0195 ms | 0.0182 ms |
| VectorVectorMatrixProd           | 4.594 ms | 0.0249 ms | 0.0233 ms |
| VectorVectorMatrixProdOptimized  | 3.026 ms | 0.0266 ms | 0.0249 ms |
| VectorVectorMatrixProdOptimized2 | 2.052 ms | 0.0346 ms | 0.0507 ms |
*/