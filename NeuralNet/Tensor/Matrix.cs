
using System.Numerics;

namespace NeuralNet.Tensor;

//TODO: consider adding information telling where zero are, such as to optimize product
public readonly struct Matrix
{
    public static readonly Matrix EMPTY = new(Array.Empty<float[]>());



    private readonly Vector<float>[] values;



    public Matrix(float[,] matrix)
    {
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);

        values = new Vector<float>[Height * (int)Math.Ceiling(Width / (float)Vector<float>.Count)];
        
        for(int i = 0; i < InternalHeight; i++)
        {
            for(int j = 0; j < InternalWidth; j++)
            {
                float[] vectorValues = new float[Vector<float>.Count];
                int length = Math.Min(Width - j*Vector<float>.Count, Vector<float>.Count);
                Buffer.BlockCopy(
                    matrix, 
                    (i * Width + j * Vector<float>.Count) * sizeof(float), 
                    vectorValues, 0, 
                    length*sizeof(float));
                values[i * InternalWidth + j] = new(vectorValues);
            }
        }
    }

    public Matrix(float[][] matrix)
    {
        Height = matrix.Length;
        Width = matrix[0].Length;

        values = new Vector<float>[Height * (int)Math.Ceiling(Width / (float)Vector<float>.Count)];
        Buffer.BlockCopy(matrix, 0, values, 0, Width * Height * sizeof(float));
    }

    public Matrix(Vector[] matrix, bool isColumns = true)
    {
        if(isColumns)
        {
            Height = matrix[0].Height;
            Width = matrix.Length;
            values = new Vector<float>[Height * (int)Math.Ceiling(Width / (float)Vector<float>.Count)];
            for(int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    float[] vectorValues = new float[Vector<float>.Count];
                    for(int k = 0; k < Vector<float>.Count; k++)
                    {
                        vectorValues[k] = matrix[j][i];
                    }
                    values[i * Width + j] = new Vector<float>(vectorValues);
                }
            }
            return;
        }

        Height = matrix.Length;
        Width = matrix[0].Height;

        values = new Vector<float>[Height * (int)Math.Ceiling(Width / (float)Vector<float>.Count)];
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
        values = new Vector<float>[Height * (int)Math.Ceiling(Width / (float)Vector<float>.Count)];
    }



    public int Height { get; }
    public int Width { get; }

    private int InternalHeight => Height;
    private int InternalWidth => (int)Math.Ceiling(Width / (float)Vector<float>.Count);



    public float this[int row, int col]
    {
        get
        {
            if(row < 0 || col < 0 || row > Height || col > Width) throw new IndexOutOfRangeException();
            return values[row * InternalWidth + col / Vector<float>.Count][col % Vector<float>.Count];
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
        for(int i = 0; i < result.InternalHeight; i++)
        {
            for(int j = 0; j < result.InternalWidth; j++)
            {
                float[] vectorValues = new float[Vector<float>.Count];

                int length = Math.Min(Height - (j * Vector<float>.Count), Vector<float>.Count);
                for(int k = 0;  k < length; k++)
                {
                    vectorValues[k] = values[j*Width + k*InternalWidth + i / Vector<float>.Count][i % Vector<float>.Count];
                }

                result.values[i*result.InternalWidth + j] = new(vectorValues);
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
        for(int i = 0; i < A.InternalHeight; i++)
        {
            for(int j = 0; j < A.InternalWidth; j++)
            {
                C.values[i*C.InternalWidth + j] = A.values[i * C.InternalWidth + j] + B.values[i * C.InternalWidth + j];
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

        Matrix B2 = B.Transpose();
        Matrix C = new(A.Height, B2.Height);

        for(int i = 0; i < A.InternalHeight; i++)
        {
            for(int j = 0; j < B.InternalWidth; j++)
            {
                // vector dot product 
                float[] value = new float[Vector<float>.Count];

                int length = Math.Min(Vector<float>.Count, C.Width - j * Vector<float>.Count);
                for(int k = 0; k < length; k++)
                {
                    // single dot product 
                    Vector<float> totalVector = new();

                    for(int l = 0; l < A.InternalWidth; l++)
                    {
                        totalVector += A.values[i*A.InternalWidth + l] * B2.values[(j * Vector<float>.Count + k)*B2.InternalWidth + l];
                    }

                    float totalFloat = 0;
                    for(int NNN = 0; NNN < Vector<float>.Count; NNN++)
                    {
                        totalFloat += totalVector[NNN];
                    }

                    value[k] = totalFloat;
                }

                C.values[i*C.InternalWidth + j] = new(value);
            }
        }

        return C;
    }

    public static Matrix operator *(Matrix A, float b)
    {
        Matrix C = new(A.Height, A.Width);
        for(int i = 0; i < A.InternalHeight; i++)
        {
            for(int j = 0; j < A.InternalWidth; j++)
            {
                C.values[i* A.InternalWidth + j] = A.values[i*A.InternalWidth + j] * b;
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

        float[] values = new float[A.Height];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = A[i, 1];
        }

        return new Vector(values);
    }



    public Matrix ConcatByWidth(Matrix right)
    {
        Matrix result = new(Math.Max(Height, right.Height), Width + right.Width);
        
        //left
        for(int i = 0; i < InternalHeight; i++)
        {
            for(int j = 0; j < Width / Vector<float>.Count; j++)
            {
                result.values[i*result.InternalWidth + j] = values[i*InternalWidth + j];
            }
        }

        //combine intermediate
        int borrow = InternalWidth*Vector<float>.Count - Width;
        
        if(borrow != 0)
        {
            for(int i = 0; i < right.InternalHeight; i++)
            {
                float[] vecValues = new float[Vector<float>.Count];

                for(int j = 0; j < Vector<float>.Count - borrow; j++)
                {
                    vecValues[j] = values[i*InternalWidth + InternalWidth - 1][j];
                }

                for(int j = 0; j < borrow; j++)
                {
                    vecValues[Vector<float>.Count - borrow + j] = right.values[i*right.InternalWidth][j];
                }

                result.values[i * result.InternalWidth + InternalWidth - 1] = new(vecValues);
            }

        }

        //right
        for(int i = 0; i < result.InternalHeight; i++)
        {
            for(int j = InternalWidth; j < result.InternalWidth; j++)
            {
                float[] vecValues = new float[Vector<float>.Count];

                for(int k = 0; k < Vector<float>.Count - borrow; k++)
                {
                    vecValues[k] = right.values[i * right.InternalWidth + j-InternalWidth][borrow + k];
                }

                int length = Math.Min(borrow, right.Width - (j-InternalWidth+1) *Vector<float>.Count);
                for(int k = 0; k < length; k++)
                {
                    vecValues[Vector<float>.Count - borrow + k] = right.values[i * right.InternalWidth + j - InternalWidth + 1][k];
                }

                result.values[i * result.InternalWidth + j] = new(vecValues);
            }
        }

        return result;
    }

    public Matrix ConcatByHeight(Matrix bottom)
    {
        Matrix result = new(Height + bottom.Height, Math.Max(Width, bottom.Width));

        //left
        for(int i = 0; i < InternalHeight; i++)
        {
            for(int j = 0; j < InternalWidth; j++)
            {
                result.values[i * result.InternalWidth + j] = values[i * InternalHeight + j];
            }
        }

        //right
        for(int i = 0; i < bottom.InternalHeight; i++)
        {
            for(int j = 0; j < bottom.InternalWidth; j++)
            {
                result.values[(i + InternalHeight) * result.InternalWidth + j] = bottom.values[i * bottom.InternalHeight + j];
            }
        }

        return result;
    }



    public Vector Flatten()
    {
        float[] values = new float[Height * Width];

        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                values[i*Width + j] = this[i, j];
            }
        }

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
}
