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
                gradient[i, i*InputSize + j] = input[j];
            }
        }

        // 1
        for(int i = 0; i < OutputSize; i++)
        {
            gradient[i, OutputSize * InputSize + i] = 1;
        }

        return Matrix.Product(
            activation.ComputeGradient(Matrix.Add(bias, Matrix.Product(matrix, input))), //TODO: figure out how to reuse this computation done by the trainer
            gradient);
    }

    public float[,] ComputeInputGradient(float[] input)
    {
        return Matrix.Product(
            activation.ComputeGradient(Matrix.Add(bias, Matrix.Product(matrix, input))), //TODO: figure out how to reuse this computation done by the trainer
                                                                                         // Alternatively, consider  removing the activation functions from the affine layer
            matrix);
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
