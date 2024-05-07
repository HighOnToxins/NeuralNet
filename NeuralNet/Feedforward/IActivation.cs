using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public interface IActivation
{

    public Vector Run(Vector input);

    //vector -> scalar-matrix
    public Vector ComputeGradient(Vector input);

}
