
namespace NeuralNet.Program;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;

    public NewtonProgram(ITrainer trainer)
    {
        this.trainer = trainer;
    }

    public string[] VariatePropertyNames 
        => new string[]{ "Loss", "Delta" };

    public string[] ConstantPropertyNames
        => Array.Empty<string>();

    public float[] ConstantProperties
        => Array.Empty<float>();

    public void InitRun(INet net) { }

    public float[] Update(INet net, int iteration)
    {
        (float[] gradient, float loss) = trainer.Train(net);
        float[] delta = gradient.Scale(loss / gradient.LengthSquared());
        net.AddWeights(delta);

        return new float[]{ loss, delta.Length() };
    }

}
