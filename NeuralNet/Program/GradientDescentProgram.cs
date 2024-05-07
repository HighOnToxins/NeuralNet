
using NeuralNet.Tensor;

namespace NeuralNet.Program;

public sealed class GradientDescentProgram : TrainingProgram
{
    private readonly ITrainer trainer;

    private readonly Func<Vector, Vector> learningScalar;

    public GradientDescentProgram(ITrainer trainer, Func<Vector, Vector> learningScalar)
    {
        this.trainer = trainer;
        this.learningScalar = learningScalar;
    }

    protected override string[] VariatePropertyNames 
        => new string[] { "Iteration", "Loss", "Speed", "Acceleration" };

    protected override string[] ConstantPropertyNames 
        => new string[] { };

    protected override float[] ConstantProperties 
        => new float[] {  };

    protected override void InitRun(INet net)
    {
    }

    protected override float[] Update(INet net, int iteration)
    {
        (Vector acceleration, float loss) = trainer.Train(net);
        Vector speed = learningScalar.Invoke(acceleration);
        
        net.AddWeights(speed);

        return new float[] { iteration, loss, speed.Length(), acceleration.Length() };
    }

}
