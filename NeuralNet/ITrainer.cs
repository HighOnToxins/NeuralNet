using NeuralNet.Tensor;

namespace NeuralNet;

public interface ITrainer<N> : ITrainer where N : INet
{

    public Vector Train(N net);

    Vector ITrainer.Train(INet net) => Train((N)net);

}

/// <summary> A trainer is the method for computing the full gradient for a network over some data. </summary>
public interface ITrainer: ITester
{

    public Vector Train(INet net);


}
