using NeuralNet.Tensor;
using NeuralNet.TrainingProgram.Testing;

namespace NeuralNet.TrainingProgram;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;
    private readonly ITester training;

    public NewtonProgram(ITrainer trainer, ITester training)
    {
        this.trainer = trainer;
        this.training = training;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector gradient = trainer.Train(net);
        float loss = training.Loss(net);
        Vector delta = gradient * (loss / gradient.LengthSquared());
        net.AddWeights(delta);
    }

}
