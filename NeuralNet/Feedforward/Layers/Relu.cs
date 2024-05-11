using NeuralNet.Tensor;

namespace NeuralNet.Feedforward.Layers;

public class ReLU: IActivation
{

    private readonly float leak;

    public ReLU(float leak = 0)
    {
        this.leak = leak;
    }

    public Vector Run(Vector input)
    {
        float[] result = new float[input.Height];
        for(int i = 0; i < input.Height; i++)
        {
            result[i] = input[i] >= 0 ? input[i] : input[i]*leak;
        }
        return new Vector(result);
    }

    public Vector ComputeGradient(Vector input)
    {
        float[] result = new float[input.Height];
        for(int i = 0; i < input.Height; i++)
        {
            result[i] = input[i] >= 0 ? 1 : -leak;
        }
        return new Vector(result);
    }
}
