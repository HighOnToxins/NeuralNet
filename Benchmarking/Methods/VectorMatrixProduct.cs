
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

        int InternalWidth = (int)Math.Ceiling(matrix.Width / (float)Vector<float>.Count);

        Vector<float>[] BValues = VectorMatrixUtil.GetValues(vector);
        Vector<float>[] AValues = VectorMatrixUtil.GetValues(matrix);

        StructVector result = new(matrix.Height);

        for(int i = 0; i < matrix.Height; i++)
        {
            Vector<float> totalVector = new();

            for(int l = 0; l < InternalWidth; l++)
            {
                totalVector += AValues[i * InternalWidth + l] * BValues[l];
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
