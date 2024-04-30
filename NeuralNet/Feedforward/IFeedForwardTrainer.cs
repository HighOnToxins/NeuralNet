namespace NeuralNet.Feedforward;

internal interface IFeedForwardTrainer
{

    public float Train(FeedForwardNet net, float learningRate, TrainingOption option = TrainingOption.Minimize);

    public float Loss(FeedForwardNet net);

    //public float Validation(FeedForwardNet net);

    //public float Accuracy(FeedForwardNet net);

}
