namespace NeuralNet.Feedforward.Layers;

public class Identity: IActivation
{
    public float[] Run(float[] input)
    {
        return input;
    }

    public float[] ComputeGradient(float[] input)
    {
        float[] result = new float[input.Length];

        for(int i = 0; i < input.Length; i++)
        {
            result[i] = 1;
        }

        return result;
    }

}