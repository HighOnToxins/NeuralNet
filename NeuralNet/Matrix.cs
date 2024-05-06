
namespace NeuralNet;

//TODO: Find more optimized product methods

public static class Matrix
{
    // MATRICES

    public static float[,] Transpose(this float[,] A)
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

    public static float[,] Product(this float[,] A, float[,] B)
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] B2 = B.Transpose();
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

    public static float[,] Add(this float[,] A, float[,] B)
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

    public static float[,] ConcatByWidth(this float[,] left, float[,] right)
    {
        float[,] result = new float[left.GetLength(0), left.GetLength(1) + right.GetLength(1)];

        for(int i = 0; i < left.GetLength(0); i++)
        {
            Buffer.BlockCopy(
                left, i * left.GetLength(1) * sizeof(float),
                result, i * result.GetLength(1) * sizeof(float),
                left.GetLength(1) * sizeof(float));
            Buffer.BlockCopy(
                right, i * right.GetLength(1) * sizeof(float),
                result, (i * result.GetLength(1) + left.GetLength(1)) * sizeof(float),
                right.GetLength(1) * sizeof(float));
        }

        return result;
    }

    public static float[,] ConcatByHeight(this float[,] top, float[,] bottom)
    {
        float[,] result = new float[top.GetLength(0) + bottom.GetLength(0), top.GetLength(1)];
        Buffer.BlockCopy(top, 0, result, 0, top.Length * sizeof(float));
        Buffer.BlockCopy(bottom, 0, result, top.Length * sizeof(float), bottom.Length * sizeof(float));
        return result;
    }


    // VECTOR-MATRIX

    public static float[] Product(this float[,] A, float[] B)
    {
        if(A.GetLength(1) != B.Length)
        {
            throw new ArgumentException("Expected a matrix and a vector such that the second length of " +
                "the matrix was equal to the length of the vector!");
        }

        float[] C = new float[A.GetLength(0)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            float total = 0;

            for(int k = 0; k < A.GetLength(1); k++)
            {
                total += A[i, k] * B[k];
            }

            C[i] = total;
        }

        return C;
    }

    public static float[,] Scale(this float[] A, float[,] B)
    {
        if(A.Length != B.GetLength(0))
        {
            throw new ArgumentException("Expected vectors to have matching lengths!");
        }

        float[,] C = new float[B.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                C[i,j] = A[i] * B[i, j];
            }
        }

        return C;
    }

    // VECTOR

    public static float[] Scale(this float[] A, float[] B)
    {
        if(A.Length != B.Length)
        {
            throw new ArgumentException("Expected vectors to have matching lengths!");
        }

        float[] C = new float[A.Length];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            C[i] = A[i] * B[i];
        }

        return C;
    }

    public static float[] Scale(this float[] A, float b)
    {
        float[] C = new float[A.Length];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            C[i] = A[i] * b;
        }

        return C;
    }

    public static float[] Add(this float[] A, float[] B)
    {
        if(A.Length != B.Length)
        {
            throw new ArgumentException("Expected vectors with equal lengths!");
        }

        float[] C = new float[A.Length];

        for(int i = 0; i < C.Length; i++)
        {
            C[i] = A[i] + B[i];
        }

        return C;
    }

    public static float LengthSquared(this float[] vector)
    {
        float lengthSquared = 0;

        for(int i = 0; i < vector.Length; i++)
        {
            lengthSquared += vector[i] * vector[i];
        }

        return lengthSquared;
    }

    public static float Length(this float[] vector)
    {
        return (float) Math.Sqrt(LengthSquared(vector));
    }

}
