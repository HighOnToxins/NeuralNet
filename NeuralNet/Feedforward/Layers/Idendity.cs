
namespace NeuralNet.Feedforward.Layers;

public class Identity: IActivation
{
    public float[,] ComputeGradient(float[] input)
    {
        float[,] result = new float[input.Length, input.Length];

        for(int i = 0; i < input.Length; i++)
        {
            result[i, i] = 1;
        }

        return result;
    }

    public float[] Run(float[] input)
    {
        return input;
    }
}