using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;

    private readonly float learningRate;

    private readonly TrainingOption option;

    public NewtonProgram(
        ITrainer trainer, 
        float learningRate,
        TrainingOption option = TrainingOption.Minimize)
    {
        this.trainer = trainer;
        this.learningRate = learningRate;

        this.option = option;
    }

    public void InitRun(INet net) { }

    public void Update(INet net)
    {
        Vector gradient = trainer.Train(net);
        float loss = trainer.Loss(net);
        Vector delta = gradient * ((float)option * learningRate * loss / gradient.LengthSquared());
        net.AddWeights(delta);
    }
}
