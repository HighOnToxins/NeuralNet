namespace NeuralNet.TrainingProgram.Display;

public sealed class LossMeasure: IMeasure
{

    private readonly ITester trainer;
    private readonly ITester tester;

    public LossMeasure(ITester trainer, ITester tester)
    {
        this.trainer = trainer;
        this.tester = tester;
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
