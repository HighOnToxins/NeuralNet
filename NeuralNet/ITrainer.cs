
namespace NeuralNet;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public interface ITrainer<N>:ITrainer where N: INet
{

    public float[] Train(N net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(N net);

    //public float Validation(FeedForwardNet net);

    //public float Accuracy(FeedForwardNet net);

}

public interface ITrainer
{

    public float[] Train(INet net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(INet net);

    //public float Validation(FeedForwardNet net);

    //public float Accuracy(FeedForwardNet net);

}
