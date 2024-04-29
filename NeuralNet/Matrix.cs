
namespace NeuralNet;

public static class Matrix
{
    public static float[] ProductFirstTransposed(float[,] A, float[] B)
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

    public static float[,] ProductFirstTransposed(float[,] A, float[,] B)
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

    public static float[,] Product(float[,] A, float[,] B)
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

    public static float[] Product(float[,] A, float[] B)
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

    public static float[,] Add(float[,] A, float[,] B)
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
                C[i, j] = A[i,j] + B[i,j];
            }
        }

        return C;
    }

    public static float[] Add(float[] A, float[] B)
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

    public static float Length(float[] vector)
    {
        float lengthSquared = 0;

        for(int i = 0; i < vector.Length; i++)
        {
            lengthSquared += vector[i] * vector[i];
        }

        return (float) Math.Sqrt(lengthSquared);
    }

    public static float[,] ConcatWidth(float[,] leftPart, float[,] rightPart)
    {
        float[,] result = new float[leftPart.GetLength(0)+rightPart.GetLength(0), leftPart.GetLength(1)+rightPart.GetLength(1)];

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
                result[i+leftPart.GetLength(0),j+rightPart.GetLength(0)] = rightPart[i, j];
            }
        }

        return result;
    }
}
