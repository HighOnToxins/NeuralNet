using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class LinearRateProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;
    private readonly float carry;

    private readonly TrainingOption option;

    public LinearRateProgram(
        ITrainer trainer, 
        float learningRate, 
        float carry = 0,
        TrainingOption option = TrainingOption.Minimize)
    {
        this.trainer = trainer;

        this.learningRate = learningRate;
        this.carry = carry;

        this.option = option;

        velocity = Vector.EMPTY;
    }

    private Vector velocity;

    public void InitRun(INet net)
    {
        velocity = new(net.GetWeightLength());
    }

    public void Update(INet net)
    {
        Vector acceleration = trainer.Train(net) * (float) option;
        velocity = velocity*carry + acceleration*learningRate;
        net.AddWeights(velocity);
    }

}
