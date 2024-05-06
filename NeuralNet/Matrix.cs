
namespace NeuralNet;

//TODO: Find more optimized product methods

public static class Matrix
{
    // MATRICES

    public static float[,] TransposeProduct(this float[,] A, float[,] B)
    {
        if(A.GetLength(0) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[A.GetLength(1), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(0); k++)
                {
                    total += A[k, i] * B[k, j];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    public static float[,] Product(this float[,] A, float[,] B)
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

    public static float[,] ConcatByWidth(this float[,] leftPart, float[,] rightPart)
    {
        float[,] result = new float[leftPart.GetLength(0), leftPart.GetLength(1) + rightPart.GetLength(1)];

        for(int i = 0; i < leftPart.GetLength(0); i++)
        {
            for(int j = 0; j < leftPart.GetLength(1); j++)
            {
                result[i, j] = leftPart[i, j];
            }
        }

        for(int i = 0; i < rightPart.GetLength(0); i++)
        {
            for(int j = 0; j < rightPart.GetLength(1); j++)
            {
                result[i, j + rightPart.GetLength(0)] = rightPart[i, j];
            }
        }

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

    public static float[] TransposeProduct(this float[,] A, float[] B)
    {
        if(A.GetLength(0) != B.Length)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] C = new float[A.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            float total = 0;

            for(int k = 0; k < A.GetLength(0); k++)
            {
                total += A[k, i] * B[k];
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
