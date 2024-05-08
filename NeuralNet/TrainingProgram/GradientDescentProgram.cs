using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class GradientDescentProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly Func<Vector, Vector> learningScalar;

    public GradientDescentProgram(ITrainer trainer, Func<Vector, Vector> learningScalar)
    {
        this.trainer = trainer;
        this.learningScalar = learningScalar;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector acceleration = trainer.Train(net);
        Vector speed = learningScalar.Invoke(acceleration);
        net.AddWeights(speed);
    }
}
