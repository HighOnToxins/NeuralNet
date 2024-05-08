
namespace NeuralNet.TrainingProgram.Display;

public sealed class IterationMeasure: IMeasure
{
    private int iterationCount;

    public IEnumerable<string> CollectFields()
    {
        iterationCount = 0;

        yield return "Iteration";
    }

    public IEnumerable<object> CollectInfo(INet net)
    {
        yield return iterationCount;

        iterationCount++;
    }
}
