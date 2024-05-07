
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FFCategoryTrainer: ITrainer<FeedforwardNet>, IEvaluator
{

    private readonly Vector[][] inputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public int CategoryCount { get; }

    public FFCategoryTrainer(Vector[][] inputs, Vector[] targets, IFeedForwardLoss loss)
    {
        CategoryCount = inputs.Length;

        if(targets.Length != CategoryCount)
        {
            throw new ArgumentException();
        }

        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public (Vector, float) Train(FeedforwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        Vector totalGradient = new(net.GetWeightLength());
        float totalLoss = 0;

        for(int i = 0; i < CategoryCount; i++)
        {
            for(int j = 0; j < inputs[i].Length; j++)
            {
                (Matrix gradient, Vector run) = net.Gradient(inputs[i][j]);
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
            for(int j = 0; j < inputs[i].Length; j++)
            {
                totalLoss += loss.Compute(targets[i], net.Run(inputs[i][j]));
            }
        }

        return totalLoss;
    }

    public Vector Guess(Vector netResult)
    {
        int maxIndex = 0;
        for(int i = 1; i < netResult.Height; i++) 
        {
            if(maxIndex < netResult[i])
            {
                maxIndex = i;
            }
        }

        float[] guessValues = new float[maxIndex];
        guessValues[maxIndex] = 1f;

        return new(guessValues);
    }

    public Vector[] CategoryData(int category)
    {
        return inputs[category];
    }

}
