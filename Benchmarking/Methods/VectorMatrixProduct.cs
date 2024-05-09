
using Benchmarking.Objects;
using System.Numerics;

namespace Benchmarking.Methods;

public static class VectorMatrixProduct
{
    public static StructVector VectorProd(StructMatrix matrix, StructVector vector)
    {
        if(matrix.Width != vector.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int internalWidth = (int)Math.Ceiling(matrix.Width / (float)Vector<float>.Count);

        Vector<float>[] matrixVectors = VectorMatrixUtil.GetValues(matrix);
        Vector<float>[] vectorVectors = VectorMatrixUtil.GetValues(vector);

        StructVector result = new(matrix.Height);

        for(int i = 0; i < matrix.Height; i++)
        {
            Vector<float> totalVector = new();

            for(int l = 0; l < internalWidth; l++)
            {
                totalVector += matrixVectors[i * internalWidth + l] * vectorVectors[l];
            }

            float totalFloat = 0;
            for(int l = 0; l < Vector<float>.Count; l++)
            {
                totalFloat += totalVector[l];
            }

            result[i] = totalFloat;
        }

        return result;
    }


    public static StructVector VectorProdOptimized(StructMatrix matrix, StructVector vector)
    {
        if(matrix.Width != vector.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int internalLength = (int)Math.Ceiling(matrix.Width / (float)Vector<float>.Count);
        Vector<float>[] vectorVectors = VectorMatrixUtil.GetValues(vector);

        StructVector result = new(matrix.Height);

        for(int i = 0; i < matrix.Height; i++)
        {
            Vector<float> totalVector = new();

            for(int l = 0; l < internalLength; l++)
            {
                int length = Math.Min(Vector<float>.Count, vector.Height - l*Vector<float>.Count);
                float[] floats = new float[length];
                Buffer.BlockCopy(matrix.values, (i * matrix.Width + l * Vector<float>.Count)*sizeof(float), floats, 0, length*sizeof(float));
                Vector<float> matrixVectors = new(floats); 

                totalVector += matrixVectors * vectorVectors[l];
            }

            float totalFloat = 0;
            for(int l = 0; l < Vector<float>.Count; l++)
            {
                totalFloat += totalVector[l];
            }

            result[i] = totalFloat;
        }

        return result;
    }

    public static StructVector VectorProdOptimized2(StructMatrix matrix, StructVector vector)
    {
        if(matrix.Width != vector.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int internalLength = (int)Math.Ceiling(matrix.Width / (float)Vector<float>.Count);
        StructVector result = new(matrix.Height);

        for(int i = 0; i < matrix.Height; i++)
        {
            Vector<float> totalVector = new();

            for(int l = 0; l < internalLength; l++)
            {
                int length = Math.Min(Vector<float>.Count, vector.Height - l * Vector<float>.Count);
                float[] floats = new float[Vector<float>.Count];

                for(int k = 0; k < length; k++)
                {
                    floats[k] = matrix[i,l*Vector<float>.Count + k] * vector[k];
                }

                totalVector += new Vector<float>(floats);
            }

            float totalFloat = 0;
            for(int l = 0; l < Vector<float>.Count; l++)
            {
                totalFloat += totalVector[l];
            }

            result[i] = totalFloat;
        }

        return result;
    }

}
