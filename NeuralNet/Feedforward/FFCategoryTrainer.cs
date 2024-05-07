
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FFCategoryTrainer: Evaluator, ITrainer<FeedforwardNet>
{

    private readonly Vector[][] trainInputs;
    private readonly Vector[][] testInputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public FFCategoryTrainer(Vector[][] trainInputs, Vector[][] testInputs, Vector[] targets, IFeedForwardLoss loss)
    {
        CategoryCount = trainInputs.Length;

        if(targets.Length != CategoryCount || testInputs.Length != CategoryCount)
        {
            throw new ArgumentException();
        }

        this.trainInputs = trainInputs;
        this.testInputs = testInputs;
        this.targets = targets;
        this.loss = loss;
    }

    public (Vector, float) Train(FeedforwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        Vector totalGradient = new(net.GetWeightLength());
        float totalLoss = 0;

        for(int i = 0; i < CategoryCount; i++)
        {
            for(int j = 0; j < trainInputs[i].Length; j++)
            {
                (Matrix gradient, Vector run) = net.Gradient(trainInputs[i][j]);
                totalGradient += gradient.Transpose() * loss.Gradient(targets[i], run);
                totalLoss += loss.Compute(targets[i], run);
            }
        }

        totalGradient *= (float)option;

        return (totalGradient, totalLoss);
    }

    public float Loss(FeedforwardNet net)
    {
        float totalLoss = 0;

        for(int i = 0; i < CategoryCount; i++)
        {
            for(int j = 0; j < trainInputs[i].Length; j++)
            {
                totalLoss += loss.Compute(targets[i], net.Run(trainInputs[i][j]));
            }
        }

        return totalLoss;
    }

    protected override Vector Guess(Vector netResult)
    {
        int maxIndex = 0;
        for(int i = 1; i < netResult.Height; i++) 
        {
            if(maxIndex < netResult[i])
            {
                maxIndex = i;
            }
        }

        float[] guessValues = new float[CategoryCount];
        guessValues[maxIndex] = 1f;

        return new(guessValues);
    }

    protected override Vector[] CategoryData(int category)
        => testInputs[category];

}
