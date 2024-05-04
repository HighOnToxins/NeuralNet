
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

    float[] ITrainer.Train(INet net, TrainingOption option) => Train((N)net, option);

    float ITrainer.Loss(INet net) => Loss((N)net);

}

public interface ITrainer
{

    public float[] Train(INet net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(INet net);

    //public float Validation(INet net);

    //public float Accuracy(INet net);

}
