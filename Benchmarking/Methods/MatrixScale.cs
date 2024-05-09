using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Objects;

namespace Benchmarking.Methods;

internal static class MatrixScale
{
    public static StructMatrix PreVector(StructMatrix A, float b)
    {
        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        Vector<float>[] AValues = VectorMatrixUtil.GetValues(A);
        Vector<float>[] BValues = VectorMatrixUtil.GetValues(A);

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < AInternalWidth; j++)
            {
                BValues[i * AInternalWidth + j] = AValues[i * AInternalWidth + j] * b;
            }
        }

        return VectorMatrixUtil.GetMatrix(A.Width, A.Height, BValues);
    }

    public static StructMatrix InVector(StructMatrix A, float b)
    {
        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        StructMatrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < AInternalWidth; j++)
            {
                int length = Math.Min(Vector<float>.Count, A.Width - j * Vector<float>.Count);
                int inputOffset = i * A.Width + j * Vector<float>.Count;

                VectorMatrixUtil.FastScale(
                    A.values, inputOffset * sizeof(float),
                    b,
                    C.values, (i * A.Width + j * Vector<float>.Count) * sizeof(float),
                    length * sizeof(float));
            }
        }

        return C;
    }

}
