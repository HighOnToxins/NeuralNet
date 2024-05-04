
namespace NeuralNet.Program;

public sealed class MomentumProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;
    private readonly float decay;

    public MomentumProgram(ITrainer trainer, float learningRate, float decay)
    {
        this.trainer = trainer;

        this.learningRate = learningRate;
        this.decay = decay;

        velocity = Array.Empty<float>();
    }

    protected override string[] VariatePropertyNames 
        => new string[] { "Iteration", "Loss", "Speed", "Acceleration" };

    protected override string[] ConstantPropertyNames 
        => new string[] { "Learning Rate", "Decay" };

    protected override float[] ConstantProperties 
        => new float[] { learningRate, decay };

    private float[] velocity;

    protected override void InitRun(INet net)
    {
        velocity = new float[net.GetWeightLength()];
    }

    protected override float[] Update(INet net, int iteration)
    {
        (float[] acceleration, float loss) = trainer.Train(net);
        for(int j = 0; j < velocity.Length; j++)
            velocity[j] = velocity[j] * decay + acceleration[j] * learningRate;
        net.AddWeights(velocity);

        return new float[] { iteration, loss, velocity.Length(), acceleration.Length() };
    }

}
