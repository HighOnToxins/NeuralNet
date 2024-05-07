
namespace NeuralNet.Tensor;

public sealed class Scalar
{
    public static readonly Matrix EMPTY = new(Array.Empty<float[]>());



    private readonly float[] values;



    public Scalar(float[] scalar)
    {
        Height = scalar.Length;
        Width = scalar.Length;

        values = new float[Height];
        Buffer.BlockCopy(scalar, 0, values, 0, values.Length * sizeof(float));
    }

    public Scalar(int size)
    {
        Height = size;
        Width = size;
        values = new float[size];
    }



    public int Height { get; }
    public int Width { get; }

    public float this[int row, int col]
    {
        get
        {
            return row == col ? values[row] : 0;
        }
    }

    public float this[int row]
    {
        get
        {
            return values[row];
        }
    }

}
