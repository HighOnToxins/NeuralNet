using NeuralNet.Tensor;

namespace NetTest;

public class MatrixTests
{
    internal static void AssertEquivalentMatrices(Matrix matrix, Matrix expected)
    {
        Assert.That(matrix.Height, Is.EqualTo(expected.Height));
        Assert.That(matrix.Width, Is.EqualTo(expected.Width));

        for(int i = 0; i < matrix.Height; i++)
        {
            for(int j = 0; j < matrix.Width; j++)
            {
                Assert.That(matrix[i, j], Is.EqualTo(expected[i, j]));
            }
        }
    }

    internal static void AssertEquivalentVectors(Vector vector, Vector expected)
    {
        Assert.That(vector.Height, Is.EqualTo(expected.Height));

        for(int i = 0; i < vector.Height; i++)
        {
            Assert.That(vector[i], Is.EqualTo(expected[i]));
        }
    }

    [Test]
    public void TestMatricesProduct()
    {
        Matrix matrixA = new(new float[,]
        {
            { 1,2},
            { 3,4},
        });

        Matrix matrixB = new(new float[,]
        {
            { 5,6},
            { 7,8},
        });

        Matrix expected = new(new float[,]
        {
            { 19, 22 },
            { 43, 50 }
        });

        Matrix matrixC = matrixA * matrixB;

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestMatricesProduct2()
    {
        Matrix matrixA = new(new float[,]
        {
            { 1, 2, 9},
            { 3, 4, 10},
        });

        Matrix matrixB = new(new float[,]
        {
            { 5, 6},
            { 7, 8},
            { 11, 12},
        });

        Matrix expected = new(new float[,]
        {
            { 118, 130 },
            { 153, 170 }
        });

        Matrix matrixC = matrixA * matrixB;

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestMatrixAdd()
    {
        Matrix matrixA = new(new float[,]
        {
            { 1,2},
            { 3,4},
        });

        Matrix matrixB = new(new float[,]
        {
            { 5,6},
            { 7,8},
        });

        Matrix expected = new(new float[,]
        {
            { 6, 8 },
            { 10, 12 }
        });

        Matrix matrixC = matrixA + matrixB;

        AssertEquivalentMatrices(matrixC, expected);
    }

    [Test]
    public void TestTransposeProduct()
    {
        Matrix matrixA = new(new float[,]
        { 
            { 1,3},
            { 2,4}
        });

        Matrix matrixB = new(new float[,]
        {
            { 5,6},
            { 7,8},
        });

        Matrix expected = new(new float[,]
        {
            { 19, 22 },
            { 43, 50 }
        });

        Matrix matrixC = matrixA.Transpose() * matrixB;

        AssertEquivalentMatrices(matrixC, expected);
    }
}