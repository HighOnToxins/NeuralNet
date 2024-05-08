using NeuralNet.Tensor;

namespace NeuralNet;

public interface ILoss
{
    public float Compute(Vector output1, Vector output2);
}