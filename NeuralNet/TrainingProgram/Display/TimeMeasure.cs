
using System.Diagnostics;

namespace NeuralNet.TrainingProgram.Display;

public sealed class TimeMeasure: IMeasure
{

    private readonly Stopwatch stopwatch;

    public TimeMeasure()
    {
        stopwatch = new();
    }

    public IEnumerable<string> CollectFields()
    {
        yield return "Current Time [date]";

        yield return "Elapsed Time [s]";

        stopwatch.Stop();
        stopwatch.Reset();
        stopwatch.Start();
    }

    public IEnumerable<object> CollectInfo(INet net)
    {
        yield return DateTime.Now;

        yield return stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
    }
}
