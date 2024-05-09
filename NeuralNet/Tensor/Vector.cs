
using System.Numerics;

namespace NeuralNet.Tensor;

public readonly struct Vector
{
    public readonly static Vector EMPTY = new(Array.Empty<float>());



    private readonly float[] values;



    public Vector(float[] vector)
    {
        Height = vector.Length;

        values = new float[Height];
        Buffer.BlockCopy(vector, 0, values, 0, values.Length * sizeof(float));
    }

    public Vector(int height)
    {
        Height = height;
        values = new float[Height];
    }



    public int Height { get; }

    public float this[int row]
    {
        get
        {
            return values[row];
        }
        private set
        {
            values[row] = value;
        }
    }

    public Vector this[Range range]
    {
        get
        {
            return new Vector(values[range]);
        }
    }


    public Matrix Transpose()
    {
        float[,] transposedValues = new float[1, Height];
        Buffer.BlockCopy(values, 0, transposedValues, 0, transposedValues.Length * sizeof(float));
        return new Matrix(transposedValues);
    }

    public static Vector operator +(Vector a, Vector b)
    {
        if(a.Height != b.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        Vector c = new(a.Height);
        for(int i = 0; i < a.Height; i++)
        {
            c[i] = a[i] + b[i];
        }

        return c;
    }

    public static Vector operator *(Matrix A, Vector b)
    {
        if(A.Width != b.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        Vector result = new(A.Height);

        for(int i = 0; i < A.Height; i++)
        {
            float totalFloat = 0;

            for(int j = 0; j < b.Height; j++)
            {
                totalFloat += A[i, j] * b[j];
            }

            result[i] = totalFloat;
        }

        return result;
    }

    public static Vector operator *(Vector a, float b)
    {
        Vector c = new(a.Height);
        for(int i = 0; i < a.Height; i++)
        {
            c[i] = a[i] * b;
        }

        return c;
    }


    public static explicit operator Matrix(Vector a)
    {
        float[,] values = new float[a.Height, 1];
        Buffer.BlockCopy(a.values, 0, values, 0, a.values.Length*sizeof(float));
        return new Matrix(values);
    }



    public Vector Scale(Vector b)
    {
        if(Height != b.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        Vector c = new(b.Height);
        for(int i = 0; i < b.Height; i++)
        {
            c[i] = this[i] * b[i];
        }

        return c;
    }

    public Matrix Scale(Matrix b)
    {
        if(Height != b.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] C = new float[b.Height, b.Width];
        for(int i = 0; i < b.Height; i++)
        {
            for(int j = 0; j < b.Width; j++)
            {
                C[i,j] = this[i] * b[i,j];
            }
        }

        return new Matrix(C);
    }



    public float LengthSquared()
    {
        float lengthSquared = 0;

        for(int i = 0; i < values.Length; i++)
        {
            lengthSquared += values[i] * values[i];
        }

        return lengthSquared;
    }

    public float Length()
    {
        return (float)Math.Sqrt(LengthSquared());
    }


    public float[] ToArray()
    {
        float[] result = new float[values.Length];
        Buffer.BlockCopy(values, 0, result, 0, values.Length * sizeof(float));
        return result;
    }

    public Vector Append(Vector bottom)
    {
        Vector result = new(values.Length + bottom.Height);
        Buffer.BlockCopy(values, 0, result.values, 0, values.Length * sizeof(float));
        Buffer.BlockCopy(bottom.values, 0, result.values, values.Length * sizeof(float), bottom.values.Length * sizeof(float));
        return result;
    }



    //TODO: add pre-vector change to matrix-vector product
    internal static Vector<float>[] GetValues(Vector A)
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

}
