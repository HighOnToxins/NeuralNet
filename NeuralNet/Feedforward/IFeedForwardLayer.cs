namespace NeuralNet.Feedforward;

public interface IFeedForwardLayer : ILayer
{
    public (float[,], float[,], float[]) Gradient(float[] input);

}
