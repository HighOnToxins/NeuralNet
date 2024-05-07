
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IFeedForwardLoss
{

    public float Compute(Vector output1, Vector output2);

    public Vector Gradient(Vector output1, Vector output2);

}
