
namespace Benchmarking;

internal static class BenchmarkingUtil
{
    public static int MatrixSize = 100;
    public static float MatrixValueRange = 10_000;

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
