using NeuralNet.Tensor;

namespace NeuralNet;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public interface ITrainer<N> : ITrainer where N : INet
{

    public Vector Train(N net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(N net);

    Vector ITrainer.Train(INet net, TrainingOption option) => Train((N)net, option);

    float ITrainer.Loss(INet net) => Loss((N)net);

}

/// <summary> A trainer is the method for computing the full gradient for a network over some data. </summary>
public interface ITrainer
{

    public Vector Train(INet net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(INet net);

}
