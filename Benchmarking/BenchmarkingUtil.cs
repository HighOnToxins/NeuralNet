
using Benchmarking.Objects;

namespace Benchmarking;

public static class BenchmarkingUtil
{
    internal const int MATRIXSIZE = 1_000;
    internal const float MATRIXVALUERANGE = 10_000;

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
        float[,] matrix = new float[MATRIXSIZE, MATRIXSIZE];
        Random random = new();
        for(int i = 0; i < MATRIXSIZE; i++)
        {
            for(int j = 0; j < MATRIXSIZE; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * MATRIXVALUERANGE - MATRIXVALUERANGE;
            }
        }
        return matrix;
    }

    public static float[] RandomVector()
    {
        float[] vector = new float[MATRIXSIZE];
        Random random = new();
        for(int i = 0; i < MATRIXSIZE; i++)
        {
            vector[i] = (float)random.NextDouble() * 2 * MATRIXVALUERANGE - MATRIXVALUERANGE;
        }
        return vector;
    }
}
