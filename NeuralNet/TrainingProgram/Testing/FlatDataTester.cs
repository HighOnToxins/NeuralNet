using NeuralNet.Feedforward;
using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram.Testing;

public sealed class FlatDataTester: ITester
{
    private readonly Vector[] inputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public FlatDataTester(Vector[] inputs, Vector[] targets, IFeedForwardLoss loss)
    {
        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public float Loss(INet net)
    {
        float totalLoss = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            totalLoss += loss.Compute(targets[i], net.Run(inputs[i]));
        }

        return totalLoss;
    }

}
