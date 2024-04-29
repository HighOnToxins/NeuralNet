namespace NeuralNet.Feedforward;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public sealed class GradientDecentTrainer: IFeedForwardTrainer
{

    private readonly float[][] trainingData;
    private readonly float[][] trainingTarget;

    private readonly IFeedForwardLoss lossFunction;

    public GradientDecentTrainer(float[][] trainingData, float[][] trainingTarget, IFeedForwardLoss lossFunction)
    {
        this.trainingData = trainingData;
        this.trainingTarget = trainingTarget;
        this.lossFunction = lossFunction;
    }

    public float Train(FeedForwardNet net, float learningRate, TrainingOption option = TrainingOption.Minimize)
    {
        float[] totalGradient = new float[net.GetWeightLength()];

        for(int i = 0; i < trainingData.Length; i++)
        {
            (float[,] gradient, float[] run) = net.ComputeGradient(trainingData[i]);

            totalGradient = Matrix.Add(
                totalGradient,
                Matrix.ProductFirstTransposed(gradient, lossFunction.ComputeGradient(trainingTarget[i], run))
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

    public float Loss(FeedForwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        float totalLoss = 0;

        for(int i = 0; i < trainingData.Length; i++)
        {
            totalLoss += lossFunction.Compute(trainingTarget[i], net.Run(trainingData[i]));
        }

        return totalLoss;
    }

    public float Validation(FeedForwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        throw new NotImplementedException();
    }

    public float Accuracy(FeedForwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        throw new NotImplementedException();
    }
}
