
using BenchmarkDotNet.Attributes;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Benchmarking;

public class NumericsBenchmarks
{

    private readonly float[,] A;
    private readonly float[,] B;

    private readonly Vector<float>[,] a;
    private readonly Vector<float>[,] b;

    public NumericsBenchmarks()
    {
        A = RandomMatrix(100, 1000);
        B = RandomMatrix(100, 1000);

        a = Assign(A);
        b = Assign(B);
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

    static Vector<float>[,] Assign(float[,] matrix)
    {
        Vector<float>[,] result = new Vector<float>[matrix.GetLength(0), matrix.GetLength(1) / Vector<float>.Count];
        for(int i = 0; i < result.GetLength(0); i++)
        {
            for(int j = 0; j < result.GetLength(1); j++)
            {
                float[] values = new float[Vector<float>.Count];
                Buffer.BlockCopy(
                    matrix, (i * matrix.GetLength(1) + j * Vector<float>.Count) * sizeof(float),
                    values, 0,
                    Vector<float>.Count * sizeof(float));
                result[i, j] = new Vector<float>(values);
            }
        }

        return result;
    }

    /*
    [Benchmark]
    public float[,] AddFloat()
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
    public Vector<float>[] AddVector()
    {
        if(a.Length != b.Length || Marshal.SizeOf(a[0]) != Marshal.SizeOf(b[0]))
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        Vector<float>[] c = new Vector<float>[A.Length];

        for(int i = 0; i < c.Length; i++)
        {
            c[i] = a[i] + b[i];
        }

        return c;
    }

    [Benchmark] 
    public float[,] ProductFloat()
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

    */


    public static Vector<float>[,] ProductVector(Vector<float>[,] a, Vector<float>[,] b)
    {
        Vector<float>[,] c = new Vector<float>[a.GetLength(0), b.GetLength(1)];

        for(int i = 0; i < c.GetLength(0); i++)
        {
            for(int j = 0; j < c.GetLength(1); j++)
            {
                Vector<float> total = new();

                for(int k = 0; k < a.GetLength(1); k++)
                {
                    total += a[i, k] * b[k, j];
                }

                c[i, j] = total;
            }
        }

        return c;
    }
}
