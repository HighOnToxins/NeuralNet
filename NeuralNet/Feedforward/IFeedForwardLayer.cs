namespace NeuralNet.Feedforward;

public interface IFeedForwardLayer : ILayer
{
    public float[,] ComputeWeightGradient(float[] input);

    public float[,] ComputeInputGradient(float[] input);

}
