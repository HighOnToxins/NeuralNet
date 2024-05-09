
using System.Numerics;
using Benchmarking.Objects;

namespace Benchmarking.Methods;

internal static class MatrixProduct
{
    public static StructMatrix InVector(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        StructMatrix B2 = B.Transpose();
        StructMatrix C = new(A.Height, B2.Height);

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                C[i, j] = VectorMatrixUtil.FastDotProduct(
                    A.values, i * A.Width * sizeof(float),
                    B2.values, j * B2.Width * sizeof(float),
                    A.Width * sizeof(float));
            }
        }

        return C;
    }

    public static StructMatrix PreVector(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        int BInternalWidth = (int)Math.Ceiling(B.Width / (float)Vector<float>.Count);
        int BTransposedInternalWidth = (int)Math.Ceiling(B.Height / (float)Vector<float>.Count);

        Vector<float>[] BValuesTransposed = VectorMatrixUtil.GetValuesTransposed(B);
        Vector<float>[] AValues = VectorMatrixUtil.GetValues(A);

        Vector<float>[] CValues = new Vector<float>[A.Height * BInternalWidth];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < BInternalWidth; j++)
            {
                // vector dot product 
                float[] value = new float[Vector<float>.Count];

                int length = Math.Min(Vector<float>.Count, B.Width - j * Vector<float>.Count);
                for(int k = 0; k < length; k++)
                {
                    // single dot product 
                    Vector<float> totalVector = new();

                    for(int l = 0; l < AInternalWidth; l++)
                    {
                        totalVector += AValues[i * AInternalWidth + l] * BValuesTransposed[(j * Vector<float>.Count + k) * BTransposedInternalWidth + l];
                    }

                    float totalFloat = 0;
                    for(int NNN = 0; NNN < Vector<float>.Count; NNN++)
                    {
                        totalFloat += totalVector[NNN];
                    }

                    value[k] = totalFloat;
                }

                CValues[i * BTransposedInternalWidth + j] = new(value);
            }
        }

        return VectorMatrixUtil.GetMatrix(A.Height, B.Width, CValues);
    }

    public static StructMatrix UsingVectorMatrix(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        VectorMatrix TransposedVectorB = new(B.Height, B.Width, B.values, true);
        VectorMatrix VectorA = new(A.Height, A.Width, A.values, false);

        VectorMatrix VectorC = new(A.Height, B.Width);

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < VectorC.InternalWidth; j++)
            {
                // vector dot product 
                float[] value = new float[Vector<float>.Count];

                int length = Math.Min(Vector<float>.Count, B.Width - j * Vector<float>.Count);
                for(int k = 0; k < length; k++)
                {
                    // single dot product 
                    Vector<float> totalVector = new();

                    for(int l = 0; l < VectorA.InternalWidth; l++)
                    {
                        totalVector += VectorA[i, l] * TransposedVectorB[j * Vector<float>.Count + k, l];
                    }

                    float totalFloat = 0;
                    for(int l = 0; l < Vector<float>.Count; l++)
                    {
                        totalFloat += totalVector[l];
                    }

                    value[k] = totalFloat;
                }

                VectorC[i, j] = new(value);
            }
        }

        return new(VectorC);
    }

    public static StructMatrix OptimizedPreVector(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        int BTransposedInternalWidth = (int)Math.Ceiling(B.Height / (float)Vector<float>.Count);

        Vector<float>[] BValuesTransposed = VectorMatrixUtil.GetValuesTransposed(B);
        Vector<float>[] AValues = VectorMatrixUtil.GetValues(A);

        StructMatrix C = new(A.Height, B.Width);

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                Vector<float> totalVector = new();

                for(int l = 0; l < AInternalWidth; l++)
                {
                    totalVector += AValues[i * AInternalWidth + l] * BValuesTransposed[j * BTransposedInternalWidth + l];
                }

                float totalFloat = 0;
                for(int l = 0; l < Vector<float>.Count; l++)
                {
                    totalFloat += totalVector[l];
                }

                C[i, j] = totalFloat;
            }
        }

        return C;
    }

}
