using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IFeedForwardLayer : ILayer
{
    //input   ->   weight gradient,   input gradient,   result
    public (Matrix, Matrix, Vector) Gradient(Vector input);

}
