
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IFeedForwardLoss: ILoss
{
    public Vector Gradient(Vector output1, Vector output2);

}
