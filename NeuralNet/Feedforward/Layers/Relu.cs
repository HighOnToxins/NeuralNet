
namespace NeuralNet.Feedforward.Layers;

public class ReLU: IActivation
{

    public float[] Run(float[] input)
    {
        float[] result = new float[input.Length];
        for(int i = 0; i < input.Length; i++)
        {
            result[i] = Math.Max(0, input[i]);
        }
        return result;
    }

    public float[,] ComputeGradient(float[] input)
    {
        float[,] result = new float[input.Length, input.Length];
        for(int i = 0; i < input.Length; i++)
        {
            result[i,i] = input[i] >= 0 ? 1 : 0;
        }
        return result;
    }
}
