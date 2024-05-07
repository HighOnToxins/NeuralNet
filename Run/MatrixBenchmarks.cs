using BenchmarkDotNet.Attributes;
using NeuralNet.Tensor;

namespace Benchmarking;

public class MatrixBenchmarks
{

    public static float[,] Transpose(float[,] A)
    {
        float[,] B = new float[A.GetLength(1), A.GetLength(0)];

        for(int i = 0; i < B.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                B[i, j] = A[j, i];
            }
        }

        return B;
    }

    private readonly float[,] A;

    private readonly float[,] B;

    public MatrixBenchmarks()
    {
        A = RandomMatrix(100, 1000);
        B = RandomMatrix(100, 1000);
    }

    static float[,] RandomMatrix(int size, int range)
    {
        float[,] matrix = new float[size, size];
        Random random = new();
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * range - range;
            }
        }
        return matrix;
    }

    [Benchmark]
    public float[,] Add1()
    {
        if(A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] Add2()
    {
        if(A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int j = 0; j < C.GetLength(1); j++)
        {
            for(int i = 0; i < C.GetLength(0); i++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] Prod1()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B[k, j];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] Prod2()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int j = 0; j < C.GetLength(1); j++)
        {
            for(int i = 0; i < C.GetLength(0); i++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] Prod3()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                for(int k = 0; k < A.GetLength(1); k++)
                {
                    C[i, j] += A[i, k] * B[j, k];
                }
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] Prod4()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] B2 = new float[B.GetLength(1), B.GetLength(0)];

        for(int i = 0; i < B2.GetLength(0); i++)
        {
            for(int j = 0; j < B2.GetLength(1); j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }


    [Benchmark]
    public float[,] Prod5()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public float[,] ConcatByWidth1()
    {
        float[,] result = new float[A.GetLength(0), A.GetLength(1) + B.GetLength(1)];

        for(int i = 0; i < A.GetLength(0); i++)
        {
            Buffer.BlockCopy(
                A, i * A.GetLength(1) * sizeof(float),
                result, i * result.GetLength(1) * sizeof(float),
                A.GetLength(1) * sizeof(float));
            Buffer.BlockCopy(
                B, i * B.GetLength(1) * sizeof(float),
                result, (i * result.GetLength(1) + A.GetLength(1)) * sizeof(float),
                B.GetLength(1) * sizeof(float));
        }

        return result;
    }

    [Benchmark]
    public float[,] ConcatByWidth2()
    {
        float[,] result = new float[A.GetLength(0), A.GetLength(1) + B.GetLength(1)];

        for(int i = 0; i < A.GetLength(0); i++)
        {
            for(int j = 0; j < A.GetLength(1); j++)
            {
                result[i, j] = A[i, j];
            }
        }

        for(int i = 0; i < B.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                result[i, B.GetLength(0)] = B[i, j];
            }
        }

        return result;
    }


    public float[,] ConcatByHeight(float[,] top, float[,] bottom)
    {
        float[,] result = new float[top.GetLength(0) + bottom.GetLength(0), top.GetLength(0)];
        Buffer.BlockCopy(
            top, 0,
            result, 0, 
            top.Length * sizeof(float));
        Buffer.BlockCopy(
            bottom, 0, 
            result, top.Length * sizeof(float), 
            bottom.Length * sizeof(float));
        return result;
    }


    [Benchmark]
    public float[,] ConcatByWidth3()
    {
        float[,] top = Transpose(A);
        float[,] bottom = Transpose(A);

        return Transpose(ConcatByHeight(top, bottom));
    }

}
