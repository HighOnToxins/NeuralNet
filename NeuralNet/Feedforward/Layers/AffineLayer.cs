using NeuralNet.Tensor;

namespace NeuralNet.Feedforward.Layers;

public sealed class AffineLayer: IFeedForwardLayer
{

    private Matrix matrix;
    private Vector bias;

    private readonly IActivation activation;

    public AffineLayer(int inputSize, int outputSize, IActivation? activation = null)
    {
        this.activation = activation ?? new Identity(); 

        InputSize = inputSize;
        OutputSize = outputSize;

        matrix = new(outputSize, inputSize);
        bias = new(outputSize);
    }

    public AffineLayer(int inputSize, int outputSize, Vector weights, IActivation? activation = null) : this(inputSize, outputSize, activation)
    {
        SetWeights(weights);
    }

    public AffineLayer(int inputSize, int outputSize, float randomRange, IActivation? activation = null) : this(inputSize, outputSize, activation)
    {
        Random random = new();

        float[,] matrixValues = new float[outputSize, inputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrixValues[i, j] = (float)random.NextDouble() * randomRange;
            }
        }
        matrix = new(matrixValues);

        float[] biasValues = new float[outputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            biasValues[i] = (float)random.NextDouble() * randomRange;
        }
        bias = new(biasValues);
    }

    public int InputSize { get; private init; }

    public int OutputSize { get; private init; }

    private Vector Transformation(Vector input)
    {
        return matrix*input + bias;
    }

    private Matrix TransformationWeightGradient(Vector input)
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

        return new Matrix(result);
    }

    public Vector Run(Vector input)
    {
        return activation.Run(Transformation(input));
    }

    public Matrix Gradient(Vector input, out Matrix inputGradient, out Vector result)
    {
        Vector transformation = Transformation(input);
        Vector activationGradient = activation.ComputeGradient(transformation);

        inputGradient = activationGradient.Scale(matrix);
        result = activation.Run(transformation);
        return activationGradient.Scale(TransformationWeightGradient(input));
    }

    public int GetWeightLength()
    {
        return (InputSize + 1) * OutputSize;
    }

    public Vector GetWeights()
    {
        return matrix.Flatten().Append(bias);
    }

    public void SetWeights(Vector newWeights)
    {
        float[,] matrixValues = new float[OutputSize, InputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrixValues[i, j] = newWeights[i * InputSize + j];
            }
        }
        matrix = new(matrixValues);

        float[] biasValues = new float[OutputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            biasValues[i] = newWeights[InputSize * OutputSize + i];
        }
        bias = new(biasValues);
    }

    public void AddWeights(Vector newWeights)
    {
        float[,] matrixValues = new float[OutputSize, InputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            for(int j = 0; j < InputSize; j++)
            {
                matrixValues[i, j] = newWeights[i * InputSize + j];
            }
        }
        matrix += new Matrix(matrixValues);

        float[] biasValues = new float[OutputSize];
        for(int i = 0; i < OutputSize; i++)
        {
            biasValues[i] = newWeights[InputSize * OutputSize + i];
        }
        bias += new Vector(biasValues);
    }
}
