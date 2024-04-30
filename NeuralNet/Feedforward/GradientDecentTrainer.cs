namespace NeuralNet.Feedforward;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public sealed class GradientDecentTrainer: IFeedForwardTrainer
{

    private readonly float[][] inputs;
    private readonly float[][] targets;

    private readonly IFeedForwardLoss loss;

    public GradientDecentTrainer(float[][] inputs, float[][] targets, IFeedForwardLoss loss)
    {
        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public float Train(FeedForwardNet net, float learningRate, TrainingOption option = TrainingOption.Minimize)
    {
        float[] totalGradient = new float[net.GetWeightLength()];

        for(int i = 0; i < inputs.Length; i++)
        {
            (float[,] gradient, float[] run) = net.Gradient(inputs[i]);

            totalGradient = Matrix.Add(
                totalGradient,
                Matrix.ProductFirstTransposed(gradient, loss.Gradient(targets[i], run))
            );
        }

        float gradientLength = Matrix.Length(totalGradient);

        for(int i = 0; i < totalGradient.Length; i++)
        {
            totalGradient[i] *= (int)option * learningRate / gradientLength;
        }

        net.AddWeights(totalGradient);

        return gradientLength;
    }

    public float Loss(FeedForwardNet net)
    {
        float totalLoss = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            totalLoss += loss.Compute(targets[i], net.Run(inputs[i]));
        }

        return totalLoss;
    }
}
