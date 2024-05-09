
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
