
namespace NeuralNet.TrainingProgram.Display;

public sealed class IterationCollector: IInfoCollector
{
    private int iterationCount;

    public IEnumerable<string> CollectFields()
    {
        iterationCount = 0;

        yield return "Iteration";
    }

    public IEnumerable<float> CollectInfo(INet net)
    {
        yield return iterationCount;

        iterationCount++;
    }
}
