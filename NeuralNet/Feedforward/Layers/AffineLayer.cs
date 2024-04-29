
namespace NeuralNet.Feedforward.Layers;

public sealed class AffineLayer: IFeedForwardLayer
{

    private readonly float[,] matrix;
    private readonly float[] bias;

    private readonly IActivation activation;

    public AffineLayer(IActivation activation, int inputSize, int outputSize)
    {
        this.activation = activation; 

        InputSize = inputSize;
        OutputSize = outputSize;

        matrix = new float[outputSize, inputSize];
        bias = new float[outputSize];
    }

    public AffineLayer(IActivation activation, int inputSize, int outputSize, float[] weights): this(activation, inputSize, outputSize)
    {
        SetWeights(weights);
    }

    public AffineLayer(IActivation activation, int inputSize, int outputSize, float randomRange) : this(activation, inputSize, outputSize)
    {
        Random random = new();

        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * randomRange;
            }
        }

        for(int i = 0; i < OutputSize; i++)
        {
            bias[i] = (float)random.NextDouble() * randomRange;
        }
    }

    public int InputSize { get; private init; }

    public int OutputSize { get; private init; }

    public float[] Run(float[] input)
    {
        return activation.Run(Matrix.Add(bias, Matrix.Product(matrix, input)));
    }

    public float[,] ComputeWeightGradient(float[] input)
    {
        float[,] gradient = new float[OutputSize, GetWeightLength()];

        // x_k
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                gradient[i + j * OutputSize, j] = input[i];
            }
        }

        // 1
        for(int i = 0; i < OutputSize; i++)
        {
            gradient[OutputSize * InputSize + i, i] = 1;
        }

        return Matrix.Product(activation.ComputeGradient(input), gradient);
    }

    public float[,] ComputeInputGradient(float[] input)
    {
        float[,] gradient = new float[OutputSize, InputSize];

        for(int i = 0; i < gradient.GetLength(0); i++)
        {
            for(int j = 0; j < gradient.GetLength(1); j++)
            {
                gradient[i,j] = matrix[i,j];
            }
        }

        return Matrix.Product(activation.ComputeGradient(input), gradient);
    }

    public int GetWeightLength()
    {
        return (InputSize + 1) * OutputSize;
    }

    public float[] GetWeights()
    {
        float[] result = new float[GetWeightLength()];

        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                result[i*InputSize + j] = matrix[i,j];
            }
        }

        for(int i = 0; i < OutputSize; i++)
        {
            result[OutputSize*InputSize + i] = bias[i];
        }

        return result;
    }

    public void SetWeights(float[] newWeights)
    {
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrix[i, j] = newWeights[i * InputSize + j];
            }
        }

        for(int i = 0; i < OutputSize; i++)
        {
            bias[i] = newWeights[InputSize * OutputSize + i];
        }
    }

    public void AddWeights(float[] newWeights)
    {
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrix[i, j] += newWeights[i * OutputSize + j];
            }
        }

        for(int i = 0; i < OutputSize; i++)
        {
            bias[i] += newWeights[InputSize * OutputSize + i];
        }
    }
}
