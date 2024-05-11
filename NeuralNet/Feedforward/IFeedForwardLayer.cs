using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IFeedForwardLayer : ILayer
{
    public Matrix Gradient(Vector input, out Matrix inputGradient, out Vector result);

}
