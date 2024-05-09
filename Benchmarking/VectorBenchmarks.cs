
using BenchmarkDotNet.Attributes;
using Benchmarking.Methods;
using Benchmarking.Objects;

namespace Benchmarking;

internal class VectorBenchmarks
{
    public readonly StructMatrix matrixA;

    public readonly StructMatrix matrixB;

    public readonly float randomNum;


    public VectorBenchmarks()
    {
        matrixA = new(BenchmarkingUtil.RandomMatrix());
        matrixB = new(BenchmarkingUtil.RandomMatrix());

        randomNum = (float)new Random().NextDouble() * 10000f;
    }



    [Benchmark]
    public StructMatrix DefaultAdd() => matrixA + matrixB;

    [Benchmark]
    public StructMatrix InVectorAdd() => MatrixAdd.InVector(matrixA, matrixB);

    [Benchmark]
    public StructMatrix PreVectorAdd() => MatrixAdd.PreVector(matrixA, matrixB);



    [Benchmark]
    public StructMatrix DefaultScale() => matrixA * randomNum;

    [Benchmark]
    public StructMatrix InVectorScale() => MatrixScale.InVector(matrixA, randomNum);

    [Benchmark]
    public StructMatrix PreVectorScale() => MatrixScale.PreVector(matrixA, randomNum);



    [Benchmark]
    public StructMatrix DefaultProd() => matrixA * matrixB;

    [Benchmark]
    public StructMatrix PreVectorProd() => MatrixProduct.PreVector(matrixA, matrixB);

    [Benchmark]
    public StructMatrix InVectorProd() => MatrixProduct.InVector(matrixA, matrixB);

    [Benchmark]
    public StructMatrix VectorMatrixProd() => MatrixProduct.UsingVectorMatrix(matrixA, matrixB);

}

/* size: 100 (before fix)
| Method         | Mean         | Error       | StdDev      | Median       |
|--------------- |-------------:|------------:|------------:|-------------:|
| DefaultAdd     |    18.546 us |   0.3669 us |   0.6231 us |    18.481 us |
| DefaultScale   |     8.804 us |   0.1720 us |   0.2520 us |     8.779 us |
| DefaultProd    | 1,377.847 us |  19.3555 us |  16.1627 us | 1,370.131 us |
| PreVectorAdd   |    66.016 us |   1.2697 us |   1.3586 us |    65.636 us |
| PreVectorScale |    63.330 us |   1.2474 us |   1.1668 us |    63.031 us |
| PreVectorProd  |   308.547 us |   6.1158 us |   7.7345 us |   305.625 us |
| InVectorAdd    |    80.565 us |   1.5524 us |   1.3762 us |    81.029 us |
| InVectorScale  |    54.138 us |   0.7499 us |   0.6262 us |    54.043 us |
| InVectorProd   | 5,566.406 us | 155.9426 us | 449.9302 us | 5,323.045 us |
*/

/* size: 500  (before fix)
| Method         | Mean         | Error        | StdDev       | Median       |
|--------------- |-------------:|-------------:|-------------:|-------------:|
| DefaultAdd     |     552.1 us |     10.28 us |      9.62 us |     551.5 us |
| DefaultScale   |     323.2 us |      5.42 us |      5.07 us |     323.3 us |
| DefaultProd    | 165,112.4 us |  2,205.06 us |  1,954.72 us | 165,562.8 us |
| PreVectorAdd   |   3,765.6 us |     90.48 us |    266.78 us |   3,752.8 us |
| PreVectorScale |   3,853.9 us |     76.78 us |    147.93 us |   3,887.7 us |
| PreVectorProd  |  40,841.6 us |  2,334.49 us |  6,883.30 us |  42,256.9 us |
| InVectorAdd    |   3,189.7 us |    220.59 us |    650.41 us |   3,053.7 us |
| InVectorScale  |   2,201.2 us |     56.84 us |    165.81 us |   2,153.4 us |
| InVectorProd   | 617,909.0 us | 10,883.53 us | 13,365.95 us | 616,641.8 us |
*/

/* size: 1000  (before fix)
| Method         | Mean         | Error      | StdDev      |
|--------------- |-------------:|-----------:|------------:|
| DefaultAdd     |     2.326 ms |  0.0336 ms |   0.0314 ms |
| DefaultScale   |     1.456 ms |  0.0238 ms |   0.0211 ms |
| DefaultProd    | 1,311.372 ms | 18.5551 ms |  17.3565 ms |
| PreVectorAdd   |    13.226 ms |  0.2627 ms |   0.3416 ms |
| PreVectorScale |    11.304 ms |  0.1913 ms |   0.2046 ms |
| PreVectorProd  |   221.525 ms |  4.2910 ms |   4.7694 ms |
| InVectorAdd    |     9.502 ms |  0.1874 ms |   0.1753 ms |
| InVectorScale  |     7.118 ms |  0.0517 ms |   0.0403 ms |
| InVectorProd   | 4,907.852 ms | 90.7938 ms | 100.9171 ms |
*/

/* size 100 (after fix)
| Method         | Mean         | Error      | StdDev     |
|--------------- |-------------:|-----------:|-----------:|
| DefaultAdd     |    12.917 us |  0.2220 us |  0.3522 us |
| DefaultScale   |     8.594 us |  0.1205 us |  0.1006 us |
| DefaultProd    | 1,630.031 us | 23.8919 us | 21.1795 us |
| PreVectorAdd   |    66.148 us |  0.8088 us |  0.6754 us |
| PreVectorScale |    63.814 us |  0.6000 us |  0.5010 us |
| PreVectorProd  |   302.989 us |  5.7470 us |  8.7763 us |
| InVectorAdd    |    80.488 us |  1.5358 us |  1.8283 us |
| InVectorScale  |    56.068 us |  1.1016 us |  1.5443 us |
| InVectorProd   | 5,256.522 us | 82.2249 us | 72.8902 us |
*/


/* size 100
| Method           | Mean       | Error    | StdDev   |
|----------------- |-----------:|---------:|---------:|
| DefaultProd      | 1,652.0 us | 25.45 us | 21.25 us |
| PreVectorProd    |   307.6 us |  6.14 us | 12.82 us |
| VectorMatrixProd |   371.4 us |  7.39 us | 11.94 us |
*/
