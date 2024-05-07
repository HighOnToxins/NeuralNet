
using NeuralNet.Tensor;

namespace NeuralNet;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public interface ITrainer<N>:ITrainer where N: INet
{

    public (Vector, float) Train(N net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(N net);

    (Vector, float) ITrainer.Train(INet net, TrainingOption option) => Train((N)net, option);

    float ITrainer.Loss(INet net) => Loss((N)net);

}

public interface ITrainer
{

    public (Vector, float) Train(INet net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(INet net);

    //public float Validation(INet net);

    //public float Accuracy(INet net);

}

//TODO: Add verification and other test stuff
