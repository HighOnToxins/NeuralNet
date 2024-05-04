using NeuralNet.Feedforward;

namespace NeuralNet;

internal interface IFeedForwardTrainer
{

    public float[] Train(FeedForwardNet net, TrainingOption option = TrainingOption.Minimize);

    public float Loss(FeedForwardNet net);

    //public float Validation(FeedForwardNet net);

    //public float Accuracy(FeedForwardNet net);

}
