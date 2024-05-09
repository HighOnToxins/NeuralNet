
using Benchmarking.Methods;
using System.Numerics;

namespace Benchmarking.Objects;

public readonly struct StructVector
{

    internal readonly float[] values;



    public StructVector(float[] vector)
    {
        Height = vector.Length;

        values = new float[Height];
        Buffer.BlockCopy(vector, 0, values, 0, values.Length * sizeof(float));
    }

    public StructVector(int height)
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
        internal set
        {
            values[row] = value;
        }
    }

    public StructVector this[Range range]
    {
        get
        {
            return new StructVector(values[range]);
        }
    }


    public StructMatrix Transpose()
    {
        float[,] transposedValues = new float[1, Height];
        Buffer.BlockCopy(values, 0, transposedValues, 0, transposedValues.Length * sizeof(float));
        return new StructMatrix(transposedValues);
    }

    public static StructVector operator +(StructVector a, StructVector b)
    {
        if(a.Height != b.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        StructVector c = new(a.Height);
        for(int i = 0; i < a.Height; i++)
        {
            c[i] = a[i] + b[i];
        }

        return c;
    }

    public static StructVector operator *(StructMatrix A, StructVector b)
    {
        if(A.Width != b.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        StructVector result = new(A.Height);

        for(int i = 0; i < A.Height; i++)
        {
            float totalFloat = 0;

            for(int j = 0; j < b.Height; j++)
            {
                totalFloat = A[i,j] + b[j];
            }

            result[i] = totalFloat;
        }

        return result;
    }

    public static StructVector operator *(StructVector a, float b)
    {
        StructVector c = new(a.Height);
        for(int i = 0; i < a.Height; i++)
        {
            c[i] = a[i] * b;
        }

        return c;
    }


    public static explicit operator StructMatrix(StructVector a)
    {
        float[,] values = new float[a.Height, 1];
        Buffer.BlockCopy(a.values, 0, values, 0, a.values.Length*sizeof(float));
        return new StructMatrix(values);
    }



    public StructVector Scale(StructVector b)
    {
        if(Height != b.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        StructVector c = new(b.Height);
        for(int i = 0; i < b.Height; i++)
        {
            c[i] = this[i] * b[i];
        }

        return c;
    }

    public StructMatrix Scale(StructMatrix b)
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

        return new StructMatrix(C);
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

    public StructVector Append(StructVector bottom)
    {
        StructVector result = new(values.Length + bottom.Height);
        Buffer.BlockCopy(values, 0, result.values, 0, values.Length * sizeof(float));
        Buffer.BlockCopy(bottom.values, 0, result.values, values.Length * sizeof(float), bottom.values.Length * sizeof(float));
        return result;
    }

}
