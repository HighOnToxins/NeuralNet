
using NeuralNet.Tensor;

namespace NeuralNet.Program;

public sealed class MomentumProgram : TrainingProgram
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

    protected override string[] VariatePropertyNames 
        => new string[] { "Iteration", "Loss", "Speed", "Acceleration" };

    protected override string[] ConstantPropertyNames 
        => new string[] { "Learning Rate", "Carry" };

    protected override float[] ConstantProperties 
        => new float[] { learningRate, carry };

    private Vector velocity;

    protected override void InitRun(INet net)
    {
        velocity = new(net.GetWeightLength());
    }

    protected override float[] Update(INet net, int iteration)
    {
        (Vector acceleration, float loss) = trainer.Train(net);
        velocity = velocity*carry + acceleration*learningRate;
        net.AddWeights(velocity);

        return new float[] { iteration, loss, velocity.Length(), acceleration.Length() };
    }

}
