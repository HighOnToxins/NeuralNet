using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IActivation
{

    public Vector Run(Vector input);

    public Scalar ComputeGradient(Vector input);

}
