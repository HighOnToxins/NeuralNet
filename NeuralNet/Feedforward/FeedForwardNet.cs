namespace NeuralNet.Feedforward;

public sealed class FeedForwardNet: INet
{
    private readonly IFeedForwardLayer[] layers;

    public int InputSize { get; private init; }

    public int OutputSize { get; private init; }

    public FeedForwardNet(params IFeedForwardLayer[] layers)
    {
        this.layers = layers;

        if (layers is null || layers.Length == 0)
        {
            throw new ArgumentException("Feedforward network expected at least one layer, but got none!");
        }

        for(int i = 0; i < layers.Length - 1; i++)
        {
            if(layers[i].OutputSize != layers[i + 1].InputSize)
            {
                throw new ArgumentException("Feedforward network expected that the input/output sizes of the layers matched!");
            }
        }

        InputSize = layers[0].InputSize;
        OutputSize = layers[^1].OutputSize;
    }

    public float[] Run(float[] input)
    {
        float[] result = layers[0].Run(input);

        for (int i = 1; i < layers.Length; i++)
        {
            result = layers[i].Run(result);
        }

        return result;
    }

    public (float[,], float[]) ComputeGradient(float[] input)
    {
        float[,] gradientResult = layers[0].ComputeWeightGradient(input);
        float[] runResult = layers[0].Run(input);

        for (int i = 1; i < layers.Length; i++)
        {
            float[,] leftPart = layers[i].ComputeWeightGradient(runResult);
            float[,] rightPart = Matrix.Product(layers[i].ComputeInputGradient(runResult), gradientResult);

            // WeightGradient + InputGradient*gradientResult
            gradientResult = Matrix.ConcatWidth(leftPart, rightPart); 
            runResult = layers[i].Run(runResult);
        }

        return (gradientResult, runResult);
    }

    public float[] GetWeights()
    {
        List<float> weights = new();

        for (int i = 0; i < layers.Length; i++)
        {
            weights.AddRange(layers[i].GetWeights());
        }

        return weights.ToArray();
    }

    public void SetWeights(float[] newWeights)
    {
        int startWeightRange;
        int endWeightRange = -1;

        for (int i = 0; i < layers.Length; i++)
        {
            startWeightRange = endWeightRange + 1;
            endWeightRange += layers[i].GetWeightLength();
            layers[i].SetWeights(newWeights[startWeightRange..endWeightRange]);
        }
    }

    public void AddWeights(float[] newWeights)
    {
        int startWeightRange;
        int endWeightRange = -1;

        for (int i = 0; i < layers.Length; i++)
        {
            startWeightRange = endWeightRange + 1;
            endWeightRange += layers[i].GetWeightLength();
            layers[i].AddWeights(newWeights[startWeightRange..endWeightRange]);
        }
    }

    public int GetWeightLength()
    {
        return layers.Select(layer => layer.GetWeightLength()).Sum();
    }
}
