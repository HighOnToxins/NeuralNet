using System.Numerics;

namespace Benchmarking.Objects;

public readonly struct StructMatrix
{
    internal readonly float[] values;



    public StructMatrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    public StructMatrix(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Height * Width];
    }

    internal StructMatrix(VectorMatrix vectorMatrix)
    {
        Height = vectorMatrix.Height;
        Width = vectorMatrix.Width;

        values = new float[Height * Width];

        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < vectorMatrix.InternalWidth; j++)
            {
                int length = Math.Min(Vector<float>.Count, Width - j * Vector<float>.Count);
                if(length > Vector<float>.Count)
                {
                    vectorMatrix[i, j].CopyTo(values, i * Width + j * Vector<float>.Count);
                    continue;
                }

                for(int k = 0; k < length; k++)
                {
                    this[i, j * Vector<float>.Count + k] = vectorMatrix[i, j][k];
                }
            }
        }
    }


    public int Height { get; }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
        internal set
        {
            values[row * Width + col] = value;
        }
    }

    public StructMatrix Transpose()
    {
        StructMatrix result = new(Width, Height);
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                result[i, j] = this[j, i];
            }
        }

        return result;
    }

    public static StructMatrix operator +(StructMatrix A, StructMatrix B)
    {
        if (A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        StructMatrix C = new(A.Height, A.Width);
        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static StructMatrix operator *(StructMatrix A, StructMatrix B)
    {
        if (A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        StructMatrix B2 = B.Transpose();
        StructMatrix C = new(A.Height, B2.Height);

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < B2.Height; j++)
            {
                float total = 0;

                for (int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    public static StructMatrix operator *(StructMatrix A, float b)
    {
        StructMatrix C = new(A.Height, A.Width);
        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i, j] * b;
            }
        }

        return C;
    }


    public string ToString(string rowSep = "\n", string colSep = "\t")
    {
        string total = "";
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                total += this[i, j].ToString() + colSep;
            }
            total += rowSep;
        }
        return total;
    }
}
