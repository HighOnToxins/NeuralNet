﻿using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FeedForwardTrainer: ITrainer<FeedforwardNet>
{

    private readonly Vector[] inputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public FeedForwardTrainer(Vector[] inputs, Vector[] targets, IFeedForwardLoss loss)
    {
        if(inputs.Length != targets.Length)
        {
            throw new ArgumentException();
        }

        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public Vector Train(FeedforwardNet net, TrainingOption option = TrainingOption.Minimize)
    {
        Vector totalGradient = new(net.GetWeightLength());

        for(int i = 0; i < inputs.Length; i++)
        {
            (Matrix gradient, Vector run) = net.Gradient(inputs[i]); //TODO: remove touple outputs
            totalGradient += gradient.Transpose() * loss.Gradient(targets[i], run);
        }

        totalGradient *= (float)option;

        return totalGradient;
    }

    public float Loss(INet net)
    {
        float total = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            total += loss.Compute(targets[i], net.Run(inputs[i]));
        }

        return total;
    }

}
