
using System.Numerics;

namespace Benchmarking.Objects;

internal readonly struct VectorMatrix
{
    private readonly Vector<float>[] values;


    public VectorMatrix(int height, int width, float[] matrixValues, bool transpose)
    {
        if (!transpose)
        {
            Height = height;
            Width = width;

            InternalHeight = Height;
            InternalWidth = (int)Math.Ceiling(Width / (float)Vector<float>.Count);

            values = new Vector<float>[InternalHeight * InternalWidth];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < InternalWidth; j++)
                {
                    float[] floats = new float[Vector<float>.Count];
                    int nonVectorIndex = j * Vector<float>.Count;
                    int length = Math.Min(Vector<float>.Count, width - nonVectorIndex);
                    Buffer.BlockCopy(matrixValues, (i * width + nonVectorIndex) * sizeof(float), floats, 0, length * sizeof(float));
                    values[i * InternalWidth + j] = new(floats);
                }
            }
            return;
        }

        Height = width;
        Width = height;

        InternalHeight = Height;
        InternalWidth = (int)Math.Ceiling(Width / (float)Vector<float>.Count);

        values = new Vector<float>[InternalHeight * InternalWidth];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < InternalWidth; j++)
            {
                float[] floats = new float[Vector<float>.Count];
                int nonVectorIndex = j * Vector<float>.Count;
                int length = Math.Min(Vector<float>.Count, height - nonVectorIndex);

                for (int k = 0; k < length; k++)
                {
                    floats[k] = matrixValues[(nonVectorIndex + k) * width + i];
                }

                values[i * InternalWidth + j] = new(floats);
            }
        }

    }

    public VectorMatrix(int height, int width)
    {
        Height = height;
        Width = width;
        InternalHeight = height;
        InternalWidth = (int)Math.Ceiling(Width / (float)Vector<float>.Count);

        values = new Vector<float>[InternalHeight * InternalWidth];
    }


    public int Height { get; }
    public int Width { get; }

    public int InternalHeight { get; }
    public int InternalWidth { get; }


    public Vector<float> this[int internalRow, int internalCol]
    {
        get
        {
            return values[internalRow * InternalWidth + internalCol];
        }
        internal set
        {
            values[internalRow * InternalWidth + internalCol] = value;
        }
    }

}
