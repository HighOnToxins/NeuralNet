
namespace NeuralNet.Feedforward;

public sealed class FeedForwardTrainer: ITrainer<FeedforwardNet>
{

    private readonly float[][] inputs;
    private readonly float[][] targets;

    private readonly IFeedForwardLoss loss;

    public FeedForwardTrainer(float[][] inputs, float[][] targets, IFeedForwardLoss loss)
    {
        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public (float[], float) Train(FeedforwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        float[] totalGradient = new float[net.GetWeightLength()];
        float totalLoss = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            (float[,] gradient, float[] run) = net.Gradient(inputs[i]);
            totalGradient = gradient.Transpose().Product(loss.Gradient(targets[i], run)).Add(totalGradient);
            totalLoss += loss.Compute(targets[i], run);
        }

        for(int i = 0; i < totalGradient.Length; i++)
        {
            totalGradient[i] *= (float)option;
        }

        return (totalGradient, totalLoss);
    }

    public float Loss(FeedforwardNet net)
    {
        float totalLoss = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            totalLoss += loss.Compute(targets[i], net.Run(inputs[i]));
        }

        return totalLoss;
    }

}
