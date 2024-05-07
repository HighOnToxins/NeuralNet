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

    [Test]
    public void TestConcat()
    {
        Matrix matrixA = new(new float[,]
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20},
            { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
            { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 },
        });

        Matrix matrixB = new(new float[,]
        {
            { 21,  32,  43, 54,  65,  76,  87,  98,  19,  110 },
            { 211, 312, 413, 514, 615, 716, 817, 918, 119, 120},
            { 221, 322, 423, 524, 625, 726, 827, 928, 129, 130 },
            { 231, 332, 433, 534, 635, 736, 837, 938, 139, 140 },
        });

        Matrix expected = new(new float[,]
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 21,  32,  43, 54,  65,  76,  87,  98,  19,  110 },
            { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 211, 312, 413, 514, 615, 716, 817, 918, 119, 120},
            { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 , 221, 322, 423, 524, 625, 726, 827, 928, 129, 130 },
            { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 231, 332, 433, 534, 635, 736, 837, 938, 139, 140 },
        });

        Matrix matrixC = matrixA.ConcatByWidth(matrixB);

        AssertEquivalentMatrices(matrixC, expected);
    }
}