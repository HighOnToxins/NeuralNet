using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class MomentumProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;
    private readonly float carry;

    public MomentumProgram(ITrainer trainer, float learningRate, float carry)
    {
        this.trainer = trainer;

        this.learningRate = learningRate;
        this.carry = carry;

        velocity = Vector.EMPTY;
    }

    private Vector velocity;

    public void InitRun(INet net)
    {
        velocity = new(net.GetWeightLength());
    }

    public void Update(INet net)
    {
        Vector acceleration = trainer.Train(net);
        velocity = velocity*carry + acceleration*learningRate;
        net.AddWeights(velocity);
    }

}
