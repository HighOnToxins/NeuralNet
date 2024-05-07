using NeuralNet.Tensor;

namespace NeuralNet.Feedforward.Layers;

public class Identity: IActivation
{
    public Vector Run(Vector input)
    {
        return input;
    }

    public Vector ComputeGradient(Vector input)
    {
        float[] result = new float[input.Height];

        for(int i = 0; i < input.Height; i++)
        {
            result[i] = 1;
        }

        return new Vector(result);
    }

}