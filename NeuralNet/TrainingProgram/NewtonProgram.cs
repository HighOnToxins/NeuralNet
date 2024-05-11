using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;

    private readonly float learningRate;

    public NewtonProgram(ITrainer trainer, float learningRate)
    {
        this.trainer = trainer;
        this.learningRate = learningRate;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector gradient = trainer.Train(net);
        float loss = trainer.Loss(net);
        Vector delta = gradient * (learningRate * loss / gradient.LengthSquared());
        net.AddWeights(delta);
    }
}
