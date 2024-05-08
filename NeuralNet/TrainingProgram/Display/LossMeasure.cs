namespace NeuralNet.TrainingProgram.Display;

public sealed class LossMeasure: IMeasure
{

    private readonly ITrainer trainer;
    private readonly ITester tester;

    public LossMeasure(ITrainer training, ITester testing)
    {
        this.trainer = training;
        this.tester = testing;
    }

    public IEnumerable<string> CollectFields()
    {
        yield return "Training Loss";
        yield return "Testing Loss";
    }

    public IEnumerable<object> CollectInfo(INet net)
    {
        yield return trainer.Loss(net);
        yield return tester.Loss(net);
    }
}
