
using Benchmarking.Objects;

namespace Benchmarking;

public static class BenchmarkingUtil
{
    internal static int MatrixSize = 100;
    internal static float MatrixValueRange = 10_000;

    public static readonly StructMatrix matrixA;

    public static readonly StructMatrix matrixB;

    public static readonly float randomNum;

    static BenchmarkingUtil()
    {
        matrixA = new(RandomMatrix());
        matrixB = new(RandomMatrix());

        randomNum = (float)new Random().NextDouble() * 10000f;
    }

    public static float[,] RandomMatrix()
    {
        float[,] matrix = new float[MatrixSize, MatrixSize];
        Random random = new();
        for(int i = 0; i < MatrixSize; i++)
        {
            for(int j = 0; j < MatrixSize; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * MatrixValueRange - MatrixValueRange;
            }
        }
        return matrix;
    }
}
