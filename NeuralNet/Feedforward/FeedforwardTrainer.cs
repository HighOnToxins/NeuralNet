﻿
using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FeedForwardTrainer: ITrainer<FeedforwardNet>
{

    private readonly Vector[] inputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public FeedForwardTrainer(Vector[] inputs, Vector[] targets, IFeedForwardLoss loss)
    {
        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public (Vector, float) Train(FeedforwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        Vector totalGradient = new(net.GetWeightLength());
        float totalLoss = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            (Matrix gradient, Vector run) = net.Gradient(inputs[i]);
            totalGradient += gradient.Transpose() * loss.Gradient(targets[i], run);
            totalLoss += loss.Compute(targets[i], run);
        }

        totalGradient *= (float)option;

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
