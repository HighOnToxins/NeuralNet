namespace NeuralNet.Feedforward.Layers;

public sealed class AffineLayer: IFeedForwardLayer
{

    private readonly float[,] matrix;
    private readonly float[] bias;

    private readonly IActivation activation;

    public AffineLayer(int inputSize, int outputSize, IActivation? activation = null)
    {
        this.activation = activation ?? new Identity(); 

        InputSize = inputSize;
        OutputSize = outputSize;

        matrix = new float[outputSize, inputSize];
        bias = new float[outputSize];
    }

    public AffineLayer(int inputSize, int outputSize, float[] weights, IActivation? activation = null) : this(inputSize, outputSize, activation)
    {
        SetWeights(weights);
    }

    public AffineLayer(int inputSize, int outputSize, float randomRange, IActivation? activation = null) : this(inputSize, outputSize, activation)
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

    private float[] Transformation(float[] input)
    {
        return Matrix.Add(bias, Matrix.Product(matrix, input));
    }

    private float[,] TransformationWeightGradient(float[] input)
    {
        float[,] result = new float[OutputSize, GetWeightLength()];

        // x_k
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                result[i, i * InputSize + j] = input[j];
            }
        }

        // 1
        for(int i = 0; i < OutputSize; i++)
        {
            result[i, OutputSize * InputSize + i] = 1;
        }

        return result;
    }

    public float[] Run(float[] input)
    {
        return activation.Run(Transformation(input));
    }

    public (float[,], float[, ], float[]) Gradient(float[] input)
    {
        float[] transformation = Transformation(input);
        float[,] activationGradient = activation.ComputeGradient(transformation); //TODO: change activation from a matrix to a vector scalar

        float[,] weightGradient = Matrix.Product(activationGradient, TransformationWeightGradient(input));
        float[,] inputGradient = Matrix.Product(activationGradient, matrix);
        float[] result = activation.Run(transformation);

        return (weightGradient, inputGradient, result);
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
