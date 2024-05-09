
using System.Numerics;

namespace NeuralNet.Tensor;

public readonly struct VectorProdMatrix
{
    public static readonly VectorProdMatrix EMPTY = new(Array.Empty<float[]>());



    private readonly float[] values;



    public VectorProdMatrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    public VectorProdMatrix(float[][] matrix)
    {
        Height = matrix.Length;
        Width = matrix[0].Length;

        values = new float[Width * Height];
        Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
    }

    public VectorProdMatrix(int height, int width)
    {
        Height = height;
        Width = width;
        values = new float[Height * Width];
    }

    internal VectorProdMatrix(VectorMatrix vectorMatrix)
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
        private set
        {
            values[row * Width + col] = value;
        }
    }

    public VectorProdMatrix this[Range rows, Range cols]
    {
        get
        {
            int rowLength = rows.Start.Value - rows.End.Value;
            VectorProdMatrix result = new(rowLength, cols.Start.Value - cols.End.Value);

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

    public VectorProdMatrix Transpose()
    {
        VectorProdMatrix result = new(Width, Height);
        for(int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                result[i, j] = this[j, i];
            }
        }

        return result;
    }

    public static VectorProdMatrix operator +(VectorProdMatrix A, VectorProdMatrix B)
    {
        if(A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        VectorProdMatrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i, j] + B[i, j];
            }
        }

        return C;
    }

    public static VectorProdMatrix operator *(VectorProdMatrix A, VectorProdMatrix B)
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

    public static VectorProdMatrix operator *(VectorProdMatrix A, float b)
    {
        VectorProdMatrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.Height; i++)
        {
            for(int j = 0; j < A.Width; j++)
            {
                C[i, j] = A[i,j] * b;
            }
        }

        return C;
    }
    public VectorProdMatrix ConcatByWidth(VectorProdMatrix right)
    {
        VectorProdMatrix result = new(Height, Width + right.Width);

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

    public VectorProdMatrix ConcatByHeight(VectorProdMatrix bottom)
    {
        VectorProdMatrix result = new(Height + bottom.Height, Width);
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



}

/*
| Method           | Mean       | Error    | StdDev   |
|----------------- |-----------:|---------:|---------:|
| DefaultProd      | 1,652.0 us | 25.45 us | 21.25 us |
| PreVectorProd    |   307.6 us |  6.14 us | 12.82 us |
| VectorMatrixProd |   371.4 us |  7.39 us | 11.94 us |
*/

/*
| Method           | Mean       | Error    | StdDev   |
|----------------- |-----------:|---------:|---------:|
| DefaultProd      | 1,607.3 ms | 30.33 ms | 28.37 ms |
| PreVectorProd    |   230.5 ms |  4.59 ms |  9.49 ms |
| VectorMatrixProd |   260.1 ms |  5.14 ms |  8.00 ms |
*/