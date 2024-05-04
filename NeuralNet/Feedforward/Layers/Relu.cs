namespace NeuralNet.Feedforward.Layers;

public class ReLU: IActivation
{

    private readonly float leak;

    public ReLU(float leak = 0)
    {
        this.leak = leak;
    }

    public float[] Run(float[] input)
    {
        float[] result = new float[input.Length];
        for(int i = 0; i < input.Length; i++)
        {
            result[i] = input[i] >= 0 ? input[i] : input[i]*leak;
        }
        return result;
    }

    public float[,] ComputeGradient(float[] input)
    {
        float[,] result = new float[input.Length, input.Length];
        for(int i = 0; i < input.Length; i++)
        {
            result[i,i] = input[i] >= 0 ? 1 : leak;
        }
        return result;
    }
}
