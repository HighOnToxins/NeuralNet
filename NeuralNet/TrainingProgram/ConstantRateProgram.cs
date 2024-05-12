using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class ConstantRateProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;
    private readonly float carry;

    public ConstantRateProgram(ITrainer trainer, float learningRate = .1f, float carry = 0)
    {
        this.trainer = trainer;
        this.learningRate = learningRate;
        this.carry = carry;

        velocity = Vector.EMPTY;
    }

    private Vector velocity;

    public void InitRun(INet net) 
    {
        velocity = new Vector(net.GetWeightLength());
    }

    public void Update(INet net)
    {
        Vector acceleration = trainer.Train(net);
        velocity = velocity*carry + acceleration * (learningRate / acceleration.Length());
        net.AddWeights(velocity);
    }
}
