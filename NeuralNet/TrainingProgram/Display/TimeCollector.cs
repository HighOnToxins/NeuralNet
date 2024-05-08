
using System.Diagnostics;

namespace NeuralNet.TrainingProgram.Display;

public sealed class TimeCollector: IInfoCollector
{

    private readonly Stopwatch stopwatch;

    public TimeCollector()
    {
        stopwatch = new();
    }

    public IEnumerable<string> CollectFields()
    {
        yield return "Elapsed Time [s]";

        stopwatch.Stop();
        stopwatch.Reset();
        stopwatch.Start();
    }

    public IEnumerable<float> CollectInfo(INet net)
    {
        yield return stopwatch.ElapsedTicks / Stopwatch.Frequency;
    }
}
