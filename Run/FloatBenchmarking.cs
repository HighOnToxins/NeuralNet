using BenchmarkDotNet.Attributes;

namespace Benchmarking;

public readonly struct ReadonlyMatrix6 //removed readonly from indexing, with properties *STAR* (13)
{
    private readonly float[] values;

    public int Height { get;  }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
    }

    public ReadonlyMatrix6(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private ReadonlyMatrix6(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private ReadonlyMatrix6(ReadonlyMatrix6 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    public static ReadonlyMatrix6 operator +(ReadonlyMatrix6 A, ReadonlyMatrix6 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[] C = new float[A.Width * A.Height];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i * A.Height + j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix6(A.Height, A.Width, C);
    }

    public static ReadonlyMatrix6 operator *(ReadonlyMatrix6 A, ReadonlyMatrix6 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i * B.Width + j] = B[j, i];
            }
        }

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j * A.Width + k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix6(A.Height, B.Width, C);
    }

}

public struct ReadonlyMatrix5 // without readonly 
{
    private readonly float[] values;

    public readonly int Height;
    public readonly int Width;

    public float this[int row, int col] => values[row * Width + col];

    public ReadonlyMatrix5(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private ReadonlyMatrix5(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private ReadonlyMatrix5(ReadonlyMatrix5 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    public static ReadonlyMatrix5 operator +(ReadonlyMatrix5 A, ReadonlyMatrix5 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[] C = new float[A.Width * A.Height];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i * A.Height + j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix5(A.Height, A.Width, C);
    }

    public static ReadonlyMatrix5 operator *(ReadonlyMatrix5 A, ReadonlyMatrix5 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i * B.Width + j] = B[j, i];
            }
        }

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j * B.Height + k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix5(A.Height, B.Width, C);
    }

}

public readonly struct ReadonlyMatrix4 //double-array
{
    private readonly float[,] values;

    public readonly int Height => values.GetLength(0);
    public readonly int Width => values.GetLength(1);

    public readonly float this[int row, int col] => values[row, col];

    public ReadonlyMatrix4(float[,] matrix)
    {
        values = new float[matrix.GetLength(0), matrix.GetLength(1)];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private ReadonlyMatrix4(ReadonlyMatrix4 matrix)
    {
        values = new float[matrix.Height, matrix.Width];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    public static ReadonlyMatrix4 operator +(ReadonlyMatrix4 A, ReadonlyMatrix4 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[,] C = new float[A.Width, A.Height];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix4(C);
    }

    public static ReadonlyMatrix4 operator *(ReadonlyMatrix4 A, ReadonlyMatrix4 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] B2 = new float[B.Height,B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i,j] = B[j, i];
            }
        }

        float[,] C = new float[A.Height, B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j , k];
                }

                C[i, j] = total;
            }
        }

        return new ReadonlyMatrix4(C);
    }
}

public readonly struct ReadonlyMatrix3 //removed readonly from indexing
{
    private readonly float[] values;

    public readonly int Height;
    public readonly int Width;

    public float this[int row, int col] => values[row * Width + col];

    public ReadonlyMatrix3(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private ReadonlyMatrix3(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private ReadonlyMatrix3(ReadonlyMatrix3 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    public static ReadonlyMatrix3 operator +(ReadonlyMatrix3 A, ReadonlyMatrix3 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[] C = new float[A.Width * A.Height];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i * A.Height + j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix3(A.Height, A.Width, C);
    }

    public static ReadonlyMatrix3 operator *(ReadonlyMatrix3 A, ReadonlyMatrix3 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i * B.Width + j] = B[j, i];
            }
        }

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j * B.Height + k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix3(A.Height, B.Width, C);
    }

    public static ReadonlyMatrix3 OperatorProd2(ReadonlyMatrix3 A, ReadonlyMatrix3 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] b2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                b2[i * B.Width + j] = B[j, i];
            }
        }

        ReadonlyMatrix3 B2 = new(B.Height, B.Width, b2);

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix3(A.Height, B.Width, C);
    }
}

public readonly struct ReadonlyMatrix2 //array-array
{
    private readonly float[][] values;

    public readonly int Height => values.Length;
    public readonly int Width => values[0].Length;

    public readonly float this[int row, int col] => values[row][col];

    public ReadonlyMatrix2(float[,] matrix)
    {
        values = new float[matrix.GetLength(0)][];
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = new float[matrix.GetLength(1)];
            Buffer.BlockCopy(
                matrix, i*matrix.GetLength(1)*sizeof(float), 
                values[i], 0, 
                values[i].Length * sizeof(float));
        }
    }

    private ReadonlyMatrix2(float[][] values)
    {
        this.values = values;
    }

    private ReadonlyMatrix2(ReadonlyMatrix2 matrix)
    {
        values = new float[matrix.values.Length][];
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = new float[matrix.values[0].Length];
            Buffer.BlockCopy(
                matrix.values[i], 0,
                values[i], 0,
                values[i].Length * sizeof(float));
        }
    }

    public static ReadonlyMatrix2 operator +(ReadonlyMatrix2 A, ReadonlyMatrix2 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[][] C = new float[A.Width][];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i][j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix2(C);
    }

    public static ReadonlyMatrix2 operator *(ReadonlyMatrix2 A, ReadonlyMatrix2 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[][] B2 = new float[B.Height][];
        for(int i = 0; i < B.Width; i++)
        {
            B2[i] = new float[B.Width];
            for(int j = 0; j < B.Height; j++)
            {
                B2[i][j] = B[j,i];
            }
        }

        float[][] C = new float[A.Height][];

        for(int i = 0; i < A.Height; i++)
        {
            C[i] = new float[B.Width];
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j][k];
                }

                C[i][j] = total;
            }
        }

        return new ReadonlyMatrix2(C);
    }

}

public readonly struct ReadonlyMatrix 
{
    private readonly float[] values;

    public readonly int Height;
    public readonly int Width;

    public readonly float this[int row, int col] => values[row * Width + col];

    public ReadonlyMatrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private ReadonlyMatrix(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private ReadonlyMatrix(ReadonlyMatrix matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    public static ReadonlyMatrix operator +(ReadonlyMatrix A, ReadonlyMatrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[] C = new float[A.Width * A.Height];

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i * A.Height + j] = A[i, j] + B[i, j];
            }
        }

        return new ReadonlyMatrix(A.Height, A.Width, C);
    }

    public static ReadonlyMatrix operator *(ReadonlyMatrix A, ReadonlyMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i * B.Width + j] = B[j, i];
            }
        }

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j * B.Height + k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix(A.Height, B.Width, C);
    }

    public static ReadonlyMatrix OperatorProd2(ReadonlyMatrix A, ReadonlyMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] b2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                b2[i * B.Width + j] = B[j, i];
            }
        }

        ReadonlyMatrix B2 = new(B.Height, B.Width, b2);

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new ReadonlyMatrix(A.Height, B.Width, C);
    }
}

public readonly struct WriteableMatrix
{
    private readonly float[] values;

    public readonly int Height;
    public readonly int Width;

    public readonly float this[int row, int col]
    { 
        get { 
            return values[row * Width + col];
        } 
        set { 
            values[row * Width + col] = value;
        }
    }

    public WriteableMatrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private WriteableMatrix(WriteableMatrix matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Width * Height];
    }

    public static WriteableMatrix operator +(WriteableMatrix A, WriteableMatrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        WriteableMatrix C = new(A);

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static WriteableMatrix operator *(WriteableMatrix A, WriteableMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        WriteableMatrix B2 = new(B.Height, B.Width);
        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        WriteableMatrix C = new(A.Height, B.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i,j] = total;
            }
        }

        return C;
    }

    public static WriteableMatrix OperatorProd2(WriteableMatrix A, WriteableMatrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2Values = new float[B.Height * A.Height];
        float[] BValues = B.values;
        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2Values[i * A.Height + j] = BValues[j * B.Width + i];
            }
        }

        WriteableMatrix C = new(A.Height, B.Width);
        float[] CValues = C.values;
        float[] AValues = A.values;
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += AValues[i * A.Width + k] * B2Values[j * A.Width + k];
                }

                CValues[i * C.Width + j] = total;
            }
        }

        return C;
    }
}

public struct WriteableMatrix2 // without readonly 
{
    private readonly float[] values;

    public int Height;
    public int Width;

    public readonly float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
        set
        {
            values[row * Width + col] = value;
        }
    }

    public WriteableMatrix2(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix2(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private WriteableMatrix2(WriteableMatrix2 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix2(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Width * Height];
    }

    public static WriteableMatrix2 operator +(WriteableMatrix2 A, WriteableMatrix2 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        WriteableMatrix2 C = new(A);

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static WriteableMatrix2 operator *(WriteableMatrix2 A, WriteableMatrix2 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        WriteableMatrix2 B2 = new(B.Height, B.Width);
        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        WriteableMatrix2 C = new(A.Height, B.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }
}

public readonly struct WriteableMatrix3 // with properties 
{
    private readonly float[] values;

    public int Height { get; }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
        set
        {
            values[row * Width + col] = value;
        }
    }

    public WriteableMatrix3(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix3(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private WriteableMatrix3(WriteableMatrix3 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix3(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Width * Height];
    }

    public static WriteableMatrix3 operator +(WriteableMatrix3 A, WriteableMatrix3 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        WriteableMatrix3 C = new(A);

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static WriteableMatrix3 operator *(WriteableMatrix3 A, WriteableMatrix3 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        WriteableMatrix3 B2 = new(B.Height, B.Width);
        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        WriteableMatrix3 C = new(A.Height, B.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    public static WriteableMatrix3 OperatorProd2(WriteableMatrix3 A, WriteableMatrix3 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B2 = new float[B.Height * B.Width];

        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i * B.Width + j] = B[j, i];
            }
        }

        float[] C = new float[A.Height * B.Width];

        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j * B.Height + k];
                }

                C[i * B.Width + j] = total;
            }
        }

        return new WriteableMatrix3(A.Height, B.Width, C);
    }

}

public struct WriteableMatrix4 // without readonly and with properties
{
    private readonly float[] values;

    public int Height { get; }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
        set
        {
            values[row * Width + col] = value;
        }
    }

    public WriteableMatrix4(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix4(int height, int width, float[] values)
    {
        Height = height;
        Width = width;
        this.values = values;
    }

    private WriteableMatrix4(WriteableMatrix4 matrix)
    {
        Height = matrix.Height;
        Width = matrix.Width;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
    }

    private WriteableMatrix4(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Width * Height];
    }

    public static WriteableMatrix4 operator +(WriteableMatrix4 A, WriteableMatrix4 B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        WriteableMatrix4 C = new(A);

        for(int i = 0; i < A.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static WriteableMatrix4 operator *(WriteableMatrix4 A, WriteableMatrix4 B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        WriteableMatrix4 B2 = new(B.Height, B.Width);
        for(int i = 0; i < B.Width; i++)
        {
            for(int j = 0; j < B.Height; j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        WriteableMatrix4 C = new(A.Height, B.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < B.Width; j++)
            {
                float total = 0;

                for(int k = 0; k < A.Width; k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }
}

public class FloatBenchmarking
{

    private readonly float[,] A;
    private readonly float[,] B;

    private readonly float[][] A2;
    private readonly float[][] B2;

    private readonly float[] A3;
    private readonly float[] B3;

    private readonly ReadonlyMatrix A4;
    private readonly ReadonlyMatrix B4;

    private readonly WriteableMatrix A5;
    private readonly WriteableMatrix B5;

    private readonly ReadonlyMatrix2 A6;
    private readonly ReadonlyMatrix2 B6;

    private readonly ReadonlyMatrix3 A7;
    private readonly ReadonlyMatrix3 B7;

    private readonly WriteableMatrix2 A8;
    private readonly WriteableMatrix2 B8;

    private readonly WriteableMatrix3 A9;
    private readonly WriteableMatrix3 B9;

    private readonly WriteableMatrix4 A10;
    private readonly WriteableMatrix4 B10;

    private readonly ReadonlyMatrix4 A11;
    private readonly ReadonlyMatrix4 B11;

    private readonly ReadonlyMatrix5 A12;
    private readonly ReadonlyMatrix5 B12;

    private readonly ReadonlyMatrix6 A13;
    private readonly ReadonlyMatrix6 B13;

    private const int SIZE = 1_000;

    public FloatBenchmarking()
    {
        A = RandomMatrix(SIZE, 1000);
        B = RandomMatrix(SIZE, 1000);
        A2 = AssignMatrix(A);
        B2 = AssignMatrix(B);
        A3 = AssignMatrix2(A);
        B3 = AssignMatrix2(B);
        A4 = new(A);
        B4 = new(B);
        A5 = new(A);
        B5 = new(B);
        A6 = new(A);
        B6 = new(B);
        A7 = new(A);
        B7 = new(B);
        A8 = new(A);
        B8 = new(B);
        A9 = new(A);
        B9 = new(B);
        A10 = new(A);
        B10 = new(B);
        A11 = new(A);
        B11 = new(B);
        A12 = new(A);
        B12 = new(B);
        A13 = new(A);
        B13 = new(B);
    }

    static float[,] RandomMatrix(int size, int range)
    {
        float[,] matrix = new float[size, size];
        Random random = new();
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * range - range;
            }
        }
        return matrix;
    }

    static float[][] AssignMatrix(float[,] matrix)
    {
        float[][] result = new float[matrix.GetLength(0)][];
        for(int i = 0; i < result.Length; i++)
        {
            result[i] = new float[matrix.GetLength(1)];
            for(int j = 0; j < result[0].Length; j++)
            {
                result[i][j] = matrix[i, j];
            }
        }
        return result;
    }

    static float[] AssignMatrix2(float[,] matrix)
    {
        float[] result = new float[matrix.GetLength(0) * matrix.GetLength(1)];
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                result[i*matrix.GetLength(1) + j] = matrix[i, j];
            }
        }
        return result;
    }

    /*
    [Benchmark]
    public float[,] Add1()
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

    [Benchmark]
    public float[][] Add2()
    {
        if(A2.Length != B2.Length || A2[0].Length != B2[0].Length)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[][] C = new float[A2.Length][];

        for(int i = 0; i < C.Length; i++)
        {
            C[i] = new float[A2[0].Length];
            for(int j = 0; j < C[0].Length; j++)
            {
                C[i][j] = A2[i][j] + B2[i][j];
            }
        }

        return C;
    }

    [Benchmark]
    public float[] Add3()
    {

        if(A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        float[] C = new float[A.GetLength(0) * A.GetLength(1)];

        for(int i = 0; i < A.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                C[i * A.GetLength(1) + j] = A3[i * A.GetLength(1) + j] + B3[i * B.GetLength(1) + j];
            }
        }

        return C;
    }

    [Benchmark]
    public ReadonlyMatrix Add4()
    {
        return A4 + B4;
    }

    [Benchmark]
    public ReadableMatrix Add5()
    {
        return A5 + B5;
    }

    [Benchmark]
    public ReadonlyMatrix2 Add6()
    {
        return A6 + B6;
    }

    [Benchmark]
    public ReadonlyMatrix3 Add7()
    {
        return A7 + B7;
    }

    [Benchmark]
    public WriteableMatrix2 Add8()
    {
        return A8 + B8;
    }

    [Benchmark]
    public WriteableMatrix3 Add9()
    {
        return A9 + B9;
    }

    /*
    [Benchmark]
    public ReadableMatrix4 Add10()
    {
        return A10 + B10;
    }

    [Benchmark]
    public ReadonlyMatrix4 Add11()
    {
        return A11 + B11;
    }

    [Benchmark]
    public ReadonlyMatrix5 Add12()
    {
        return A12 + B12;
    }

    [Benchmark]
    public ReadonlyMatrix6 Add13()
    {
        return A13 + B13;
    }
    */

    [Benchmark]
    public float[,] Prod1()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] B2 = new float[B.GetLength(1),B.GetLength(0)];

        for(int i = 0; i < B2.GetLength(0); i++)
        {
            for(int j = 0; j < B2.GetLength(1); j++)
            {
                B2[i, j] = B[j, i];
            }
        }

        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public float[][] Prod2()
    {
        if(A2[0].Length != B2.Length)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[][] B22 = new float[B2.Length][];

        for(int i = 0; i < B22.Length; i++)
        {
            B22[i] = new float[B2[0].Length];
            for(int j = 0; j < B22[0].Length; j++)
            {
                B22[i][j] = B2[j][i];
            }
        }

        float[][] C = new float[A2.Length][];

        for(int i = 0; i < C.Length; i++)
        {
            C[i] = new float[A2[0].Length];
            for(int j = 0; j < C[0].Length; j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A2[i][k] * B2[j][k];
                }

                C[i][j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public float[] Prod3()
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[] B32 = new float[B.GetLength(0) * B.GetLength(1)];

        for(int i = 0; i < B.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                B32[i * B.GetLength(1) + j] = B3[j * B.GetLength(0) + i];
            }
        }

        float[] C = new float[A.GetLength(0) * B.GetLength(1)];

        for(int i = 0; i < A.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A3[i*A.GetLength(1) + k] * B3[j*B.GetLength(0) + k];
                }

                C[i * A.GetLength(1) + j] = total;
            }
        }

        return C;
    }

    [Benchmark]
    public ReadonlyMatrix Prod4()
    {
        return A4 * B4;
    }

    [Benchmark]
    public ReadonlyMatrix Prod42()
    {
        return ReadonlyMatrix.OperatorProd2(A4, B4);
    }

    [Benchmark]
    public WriteableMatrix Prod5()
    {
        return A5 * B5;
    }

    [Benchmark]
    public WriteableMatrix Prod52()
    {
        return WriteableMatrix.OperatorProd2(A5, B5);
    }

    [Benchmark]
    public ReadonlyMatrix2 Prod6()
    {
        return A6 * B6;
    }

    [Benchmark]
    public ReadonlyMatrix3 Prod7()
    {
        return A7 * B7;
    }

    [Benchmark]
    public WriteableMatrix2 Prod8()
    {
        return A8 * B8;
    }

    [Benchmark]
    public WriteableMatrix3 Prod9()
    {
        return A9 * B9;
    }

    [Benchmark]
    public WriteableMatrix3 Prod92()
    {
        return WriteableMatrix3.OperatorProd2(A9, B9);
    }

    [Benchmark]
    public WriteableMatrix4 Prod10()
    {
        return A10 * B10;
    }

    [Benchmark]
    public ReadonlyMatrix4 Prod11()
    {
        return A11 * B11;
    }

    [Benchmark]
    public ReadonlyMatrix5 Prod12()
    {
        return A12 * B12;
    }

    [Benchmark]
    public ReadonlyMatrix6 Prod13()
    {
        return A13 * B13;
    }

}

/* size : 100
| Method | Mean        | Error     | StdDev     | Median      |
|------- |------------:|----------:|-----------:|------------:|
| Add7   |    16.51 us |  0.327 us |   0.646 us |    16.37 us |
| Add8   |    19.55 us |  0.303 us |   0.284 us |    19.46 us |
| Add9   |    19.82 us |  0.425 us |   1.221 us |    19.25 us |
| Add13  |    17.01 us |  0.599 us |   1.679 us |    16.38 us |
| Prod7  | 1,199.42 us | 30.986 us |  87.902 us | 1,173.68 us |
| Prod8  | 1,532.15 us | 56.057 us | 165.284 us | 1,492.88 us |
| Prod9  | 1,367.44 us | 10.271 us |   8.577 us | 1,366.18 us |
| Prod92 | 1,113.50 us | 10.859 us |   9.626 us | 1,112.56 us | *
| Prod10 | 1,364.38 us |  5.391 us |   4.502 us | 1,363.78 us |
| Prod11 | 3,200.32 us |  9.947 us |   8.818 us | 3,200.03 us |
| Prod12 | 1,115.08 us |  5.915 us |   5.243 us | 1,114.52 us | *
| Prod13 | 1,111.10 us |  2.293 us |   1.915 us | 1,112.11 us | *
*/