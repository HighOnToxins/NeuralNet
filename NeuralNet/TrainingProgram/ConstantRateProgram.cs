using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class ConstantRateProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;

    public ConstantRateProgram(ITrainer trainer, float learningRate = .1f)
    {
        this.trainer = trainer;
        this.learningRate = learningRate;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector acceleration = trainer.Train(net);
        Vector speed = acceleration * (learningRate / acceleration.Length());
        net.AddWeights(speed);
    }
}
