using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IFeedForwardLayer : ILayer
{
    public void Gradient(Vector input, out Matrix weightGradient, out Matrix inputGradient, out Vector result);

}
