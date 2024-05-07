
using NeuralNet.Tensor;

namespace NeuralNet.Program;

public sealed class NewtonProgram: TrainingProgram
{

    private readonly ITrainer trainer;

    public NewtonProgram(ITrainer trainer)
    {
        this.trainer = trainer;
    }

    protected override string[] VariatePropertyNames 
        => new string[]{ "Iteration", "Loss", "Delta" };

    protected override string[] ConstantPropertyNames
        => Array.Empty<string>();

    protected override float[] ConstantProperties
        => Array.Empty<float>();

    protected override void InitRun(INet net) { }

    protected override float[] Update(INet net, int iteration)
    {
        (Vector gradient, float loss) = trainer.Train(net);
        Vector delta = gradient * (loss / gradient.LengthSquared());
        net.AddWeights(delta);

        return new float[]{ iteration, loss, delta.Length() };
    }

}
