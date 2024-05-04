using NeuralNet;

namespace NetTest;

public class MatrixTests
{
    internal static void AssertEquivalentMatrices(float[,] matrix, float[,] expected)
    {
        Assert.That(matrix.GetLength(0), Is.EqualTo(expected.GetLength(0)));
        Assert.That(matrix.GetLength(1), Is.EqualTo(expected.GetLength(1)));

        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                Assert.That(matrix[i,j], Is.EqualTo(expected[i, j]));
            }
        }
    }

    [Test]
    public void TestMatricesProduct()
    {
        float[,] matrixA =
        {
            { 1,2},
            { 3,4},
        };

        float[,] matrixB =
        {
            { 5,6},
            { 7,8},
        };

        float[,] expected =
        {
            { 19, 22 },
            { 43, 50 }
        };

        float[,] matrixC = Matrix.Product(matrixA, matrixB);

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestMatricesProduct2()
    {
        float[,] matrixA =
        {
            { 1, 2, 9},
            { 3, 4, 10},
        };

        float[,] matrixB =
        {
            { 5, 6},
            { 7, 8},
            { 11, 12},
        };

        float[,] expected =
        {
            { 118, 130 },
            { 153, 170 }
        };

        float[,] matrixC = Matrix.Product(matrixA, matrixB);

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestMatrixAdd()
    {
        float[,] matrixA =
        {
            { 1,2},
            { 3,4},
        };

        float[,] matrixB =
        {
            { 5,6},
            { 7,8},
        };

        float[,] expected =
        {
            { 6, 8 },
            { 10, 12 }
        };

        float[,] matrixC = Matrix.Add(matrixA, matrixB);

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestTransposeProduct()
    {
        float[,] matrixA =
        {
            { 1,3},
            { 2,4},
        };

        float[,] matrixB =
        {
            { 5,6},
            { 7,8},
        };

        float[,] expected =
        {
            { 19, 22 },
            { 43, 50 }
        };

        float[,] matrixC = Matrix.TransposeProduct(matrixA, matrixB);

        AssertEquivalentMatrices(matrixC, expected);
    }
}