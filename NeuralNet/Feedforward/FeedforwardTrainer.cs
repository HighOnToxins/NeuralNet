﻿namespace NeuralNet.Feedforward;

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

    public float[] Train(FeedForwardNet net, TrainingOption option = TrainingOption.Minimize)
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

        for(int i = 0; i < totalGradient.Length; i++)
        {
            totalGradient[i] *= (float)option;
        }

        return totalGradient;
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