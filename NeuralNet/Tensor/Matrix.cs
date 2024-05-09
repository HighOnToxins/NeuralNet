
using System.Numerics;

namespace NeuralNet.Tensor;

//TODO: consider adding information telling where zero are, such as to optimize product
public readonly struct Matrix
{
    public static readonly Matrix EMPTY = new(Array.Empty<float[]>());



    private readonly float[] values;



    public Matrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    public Matrix(float[][] matrix)
    {
        Height = matrix.Length;
        Width = matrix[0].Length;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    public Matrix(Vector[] matrix, bool isColumns = true)
    {
        if(isColumns)
        {
            Height = matrix[0].Height;
            Width = matrix.Length;
            values = new float[Width * Height];
            for(int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    values[i * Width + j] = matrix[j][i];
                }
            }
            return;
        }

        Height = matrix.Length;
        Width = matrix[0].Height;

        values = new float[Width * Height];
        for(int i = 0; i < Height; i++)
        {
            float[] values = matrix[i].ToArray();
            Buffer.BlockCopy(values, 0, this.values, i * Width * sizeof(float), Width * sizeof(float));
        }
    }

    public Matrix(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Height * Width];
    }



    public int Height { get; }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return values[row * Width + col];
        }
        private set
        {
            values[row * Width + col] = value;
        }
    }

    public Matrix this[Range rows, Range cols]
    {
        get
        {
            int rowLength = rows.Start.Value - rows.End.Value;
            Matrix result = new(rowLength, cols.Start.Value - cols.End.Value);

            for(int i = 0; i < result.Height; i++)
            {
                Buffer.BlockCopy(
                    values, (i * Width + rows.Start.Value) * sizeof(float),
                    result.values, i * result.Width * sizeof(float),
                    rowLength * sizeof(float));
            }

            return result;
        }
    }

    public Matrix Transpose()
    {
        Matrix result = new(Width, Height);
        for(int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                result[i, j] = this[j, i];
            }
        }

        return result;
    }

    public static Matrix operator +(Matrix A, Matrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        Matrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static Matrix operator *(Matrix A, Matrix B)
    {
        if(A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        int BTransposedInternalWidth = (int)Math.Ceiling(B.Height / (float)Vector<float>.Count);

        Vector<float>[] BValuesTransposed = GetValuesTransposed(B);
        Vector<float>[] AValues = GetValues(A);

        Matrix C = new(A.Height, B.Width);

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

    public static Matrix operator *(Matrix A, float b)
    {
        Matrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i,j] * b;
            }
        }

        return C;
    }

    public static explicit operator Vector(Matrix A)
    {
        if(A.Width != 1)
        {
            throw new ArgumentException($"Matrix must have width of 1 " +
                $"in order to be cast into a vector, had a width of {A.Width}!");
        }

        return new Vector(A.values);
    }



    public Matrix ConcatByWidth(Matrix right)
    {
        Matrix result = new(Height, Width + right.Width);

        int maxHeight = Math.Max(Height, right.Height);
        for(int i = 0; i < maxHeight; i++)
        {
            Buffer.BlockCopy(
                values, i * Width * sizeof(float),
                result.values, i * result.Width * sizeof(float),
                Width * sizeof(float));
            Buffer.BlockCopy(
                right.values, i * right.Width * sizeof(float),
                result.values, (i * result.Width + Width) * sizeof(float),
                right.Width * sizeof(float));
        }

        return result;
    }

    public Matrix ConcatByHeight(Matrix bottom)
    {
        Matrix result = new(Height + bottom.Height, Width);
        Buffer.BlockCopy(
            values, 0,
            result.values, 0, 
            values.Length * sizeof(float));
        Buffer.BlockCopy(
            bottom.values, 0, 
            result.values, values.Length * sizeof(float), 
            bottom.values.Length * sizeof(float));
        return result;
    }



    public Vector Flatten()
    {
        return new Vector(values);
    }


    public string ToString(string rowSep = "\n", string colSep = "\t")
    {
        string total = "";
        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                total += this[i,j].ToString() + colSep;
            }
            total += rowSep;
        }
        return total;
    }



    internal static Vector<float>[] GetValues(Matrix A)
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

    internal static Vector<float>[] GetValuesTransposed(Matrix A)
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

}
