
using System.Numerics;
using Benchmarking.Objects;

namespace Benchmarking.Methods;

internal static class MatrixAdd
{

    public static StructMatrix PreVector(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        Vector<float>[] AValues = VectorMatrixUtil.GetValues(A);
        Vector<float>[] BValues = VectorMatrixUtil.GetValues(B);

        Vector<float>[] CValues = new Vector<float>[A.Height * AInternalWidth];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < AInternalWidth; j++)
            {
                CValues[i * AInternalWidth + j] = AValues[i * AInternalWidth + j] + BValues[i * AInternalWidth + j];
            }
        }

        return VectorMatrixUtil.GetMatrix(A.Width, A.Height, CValues);
    }


    public static StructMatrix InVector(StructMatrix A, StructMatrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        StructMatrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < AInternalWidth; j++)
            {
                int length = Math.Min(Vector<float>.Count, A.Width - j * Vector<float>.Count);
                int offset = (i * A.Width + j * Vector<float>.Count) * sizeof(float);

                VectorMatrixUtil.FastAdd<float>(
                    A.values, offset,
                    B.values, offset,
                    C.values, offset,
                    length * sizeof(float));
            }
        }

        return C;
    }

}
