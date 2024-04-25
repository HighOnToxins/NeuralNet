namespace NeuralNet.Feedforward;

public sealed class FeedForwardNet : INet
{
    private readonly IFeedForwardLayer[] layers;

    public FeedForwardNet(params IFeedForwardLayer[] layers)
    {
        this.layers = layers;

        if (layers is null || layers.Length == 0)
        {
            throw new ArgumentException("Feedforward network expected at least one layer, but got none!");
        }
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
            // WeightGradient + InputGradient*gradientResult
            gradientResult = Matrix.Add(
                layers[i].ComputeWeightGradient(runResult),
                Matrix.Product(layers[i].ComputeInputGradient(runResult), gradientResult)
            );
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
