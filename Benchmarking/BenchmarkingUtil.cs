
using Benchmarking.Objects;

namespace Benchmarking;

public static class BenchmarkingUtil
{
    internal static int MatrixSize = 100;
    internal static float MatrixValueRange = 10_000;

    public static readonly StructMatrix matrixA;
    public static readonly StructMatrix matrixB;

    public static readonly StructVector vectorA;

    public static readonly float randomNum;

    static BenchmarkingUtil()
    {
        matrixA = new(RandomMatrix());
        matrixB = new(RandomMatrix());

        vectorA = new(RandomVector());

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

    public static float[] RandomVector()
    {
        float[] vector = new float[MatrixSize];
        Random random = new();
        for(int i = 0; i < MatrixSize; i++)
        {
            vector[i] = (float)random.NextDouble() * 2 * MatrixValueRange - MatrixValueRange;
        }
        return vector;
    }
}
