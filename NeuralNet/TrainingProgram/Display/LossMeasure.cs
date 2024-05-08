
using NeuralNet.TrainingProgram.Testing;

namespace NeuralNet.TrainingProgram.Display;

public sealed class LossMeasure: IMeasure
{

    private readonly ITester training;
    private readonly ITester testing;

    public LossMeasure(ITester training, ITester testing)
    {
        this.training = training;
        this.testing = testing;
    }

    public IEnumerable<string> CollectFields()
    {
        yield return "Training Loss";
        yield return "Testing Loss";
    }

    public IEnumerable<object> CollectInfo(INet net)
    {
        yield return training.Loss(net);
        yield return testing.Loss(net);
    }
}
