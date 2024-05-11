
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FeedforwardNet: INet
{
    private readonly IFeedForwardLayer[] layers;

    public int InputSize { get; private init; }

    public int OutputSize { get; private init; }

    public FeedforwardNet(params IFeedForwardLayer[] layers)
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

    public FeedforwardNet(float randomRange, params IFeedForwardLayer[] layers): this(layers)
    {
        Randomize(randomRange);
    }

    public FeedforwardNet(Vector weights, params IFeedForwardLayer[] layers) : this(layers)
    {
        SetWeights(weights);
    }

    public void Randomize(float randomRange)
    {
        Random random = new();
        float[] weights = new float[GetWeightLength()];
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = (float)random.NextDouble() * randomRange;
        }
        SetWeights(new Vector(weights));
    }

    public void Randomize(Func<float, float> distribution)
    {
        Random random = new();
        float[] weights = new float[GetWeightLength()];
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = distribution.Invoke((float)random.NextDouble());
        }
        SetWeights(new Vector(weights));
    }

    public Vector Run(Vector input)
    {
        Vector result = layers[0].Run(input);

        for (int i = 1; i < layers.Length; i++)
        {
            result = layers[i].Run(result);
        }

        return result;
    }

    public Matrix Gradient(Vector input, out Vector result)
    {
        layers[0].Gradient(input, 
            out Matrix gradientResult, 
            out _, 
            out result);

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].Gradient(result, 
                out Matrix weightGradient, 
                out Matrix inputGradient, 
                out result);
            Matrix rightPart = inputGradient * gradientResult;

            // WeightGradient + InputGradient*gradientResult
            gradientResult = weightGradient.ConcatByWidth(rightPart); 
        }

        return gradientResult;
    }

    public Vector GetWeights()
    {
        List<float> weights = new();

        for (int i = 0; i < layers.Length; i++)
        {
            weights.AddRange(layers[i].GetWeights().ToArray());
        }

        return new(weights.ToArray());
    }

    public void SetWeights(Vector newWeights)
    {
        int start;
        int end = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            start = end;
            end += layers[i].GetWeightLength();
            layers[i].SetWeights(newWeights[start..end]);
        }
    }

    public void AddWeights(Vector newWeights)
    {
        int start;
        int end = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            start = end;
            end += layers[i].GetWeightLength();
            layers[i].AddWeights(newWeights[start..end]);
        }
    }

    public int GetWeightLength()
    {
        return layers.Select(layer => layer.GetWeightLength()).Sum();
    }

    public void Save(string path)
    {
        BinaryWriter writer = new(File.Create(path + ".bin"));

        Vector weights = GetWeights();
        for(int i = 0; i < weights.Height; i++)
        {
            writer.Write(weights[i]);
        }

        writer.Close();
    }

    public void Load(string path)
    {
        BinaryReader reader = new(File.OpenRead(path + ".bin"));

        float[] weights = new float[GetWeightLength()];
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = reader.ReadSingle();
        }

        SetWeights(new Vector(weights));

        reader.Close();
    }

    public INet Clone()
    {
        return new FeedforwardNet(GetWeights(), layers);
    }
}
