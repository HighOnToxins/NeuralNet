
using System.Numerics;
using Benchmarking.Objects;

namespace Benchmarking.Methods;

internal static class VectorMatrixUtil
{

    internal static Vector<float>[] GetValues(StructVector A)
    {
        int internalLength = (int)Math.Ceiling(A.Height / (float)Vector<float>.Count);
        Vector<float>[] values = new Vector<float>[A.Height * internalLength];

        for(int i = 0; i < internalLength; i++)
        {
            float[] floats = new float[Vector<float>.Count];
            int nonVectorIndex = i * Vector<float>.Count;
            int length = Math.Min(Vector<float>.Count, A.Height - nonVectorIndex);
            Buffer.BlockCopy(A.values, nonVectorIndex * sizeof(float), floats, 0, length * sizeof(float));
            values[i] = new(floats);
        }

        return values;
    }
    public static Vector<float>[] GetValues(StructMatrix A)
    {
        int internalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        Vector<float>[] values = new Vector<float>[A.Height * internalWidth];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < internalWidth; j++)
            {
                float[] floats = new float[Vector<float>.Count];
                int nonVectorIndex = j * Vector<float>.Count;
                int length = Math.Min(Vector<float>.Count, A.Width - nonVectorIndex);
                Buffer.BlockCopy(A.values, (i * A.Width + nonVectorIndex) * sizeof(float), floats, 0, length * sizeof(float));
                values[i * internalWidth + j] = new(floats);
            }
        }

        return values;
    }

    public static Vector<float>[] GetValuesTransposed(StructMatrix A)
    {
        int internalWidth = (int)Math.Ceiling(A.Height / (float)Vector<float>.Count);
        Vector<float>[] values = new Vector<float>[A.Width * internalWidth];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < internalWidth; j++)
            {
                float[] floats = new float[Vector<float>.Count];
                int nonVectorIndex = j * Vector<float>.Count;
                int length = Math.Min(Vector<float>.Count, A.Height - nonVectorIndex);

                for(int k = 0; k < length; k++)
                {
                    floats[k] = A[nonVectorIndex + k, i];
                }

                values[i * internalWidth + j] = new(floats);
            }
        }

        return values;
    }

    public static StructMatrix GetMatrix(int height, int width, Vector<float>[] values)
    {
        int internalWidth = (int)Math.Ceiling(width / (float)Vector<float>.Count);
        StructMatrix result = new(height, width);

        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < internalWidth; j++)
            {
                int length = width - j * Vector<float>.Count;
                if(length > Vector<float>.Count)
                {
                    values[i * internalWidth + j].CopyTo(result.values, i * width + j * Vector<float>.Count);
                    continue;
                }

                for(int k = 0; k < length; k++)
                {
                    result[i, j * Vector<float>.Count + k] = values[i * internalWidth + j][k];
                }
            }
        }

        return result;
    }


    public static void FastAdd<T>(Array src1, int srcOffset1, Array src2, int srcOffset2, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];
        T[] BValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);
        Buffer.BlockCopy(src2, srcOffset2, BValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorB = new(BValues);

        Vector<T> vectorResult = vectorA + vectorB;

        T[] CValues = new T[Vector<float>.Count];
        vectorResult.CopyTo(CValues);
        Buffer.BlockCopy(CValues, 0, dst, dstOffset, count);
    }

    public static void FastMultiply<T>(Array src1, int srcOffset1, Array src2, int srcOffset2, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];
        T[] BValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);
        Buffer.BlockCopy(src2, srcOffset2, BValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorB = new(BValues);

        Vector<T> vectorResult = vectorA * vectorB;

        T[] resultValues = new T[Vector<T>.Count];
        vectorResult.CopyTo(resultValues);
        Buffer.BlockCopy(resultValues, 0, dst, dstOffset, count);
    }

    public static void FastScale<T>(Array src1, int srcOffset1, T scalar, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorResult = vectorA * scalar;

        T[] resultValues = new T[Vector<T>.Count];
        vectorResult.CopyTo(resultValues);
        Buffer.BlockCopy(resultValues, 0, dst, dstOffset, count);
    }

    public static float FastDotProduct(float[] src1, int srcOffset1, float[] src2, int srcOffset2, int count)
    {
        int valueCount = count / sizeof(float);
        int internalLength = (int)Math.Ceiling(valueCount / (float)Vector<float>.Count);

        Vector<float> totalVector = new();

        for(int l = 0; l < internalLength; l++)
        {
            int length = Math.Min(Vector<float>.Count, valueCount - l * Vector<float>.Count) * sizeof(float);

            float[] values1 = new float[Vector<float>.Count];
            float[] values2 = new float[Vector<float>.Count];

            Buffer.BlockCopy(src1, srcOffset1 + l * Vector<float>.Count, values1, 0, length);
            Buffer.BlockCopy(src2, srcOffset2 + l * Vector<float>.Count, values2, 0, length);

            Vector<float> vector1 = new(values1);
            Vector<float> vector2 = new(values2);

            totalVector += vector1 * vector2;
        }

        float totalFloat = 0;
        for(int l = 0; l < Vector<float>.Count; l++)
        {
            totalFloat += totalVector[l];
        }

        return totalFloat;
    }

}
