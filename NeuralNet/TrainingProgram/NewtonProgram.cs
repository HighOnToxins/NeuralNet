using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;

    public NewtonProgram(ITrainer trainer)
    {
        this.trainer = trainer;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector gradient = trainer.Train(net);
        float loss = trainer.Loss(net);

        Vector delta = gradient * (loss / gradient.LengthSquared());
        net.AddWeights(delta);
    }

}
