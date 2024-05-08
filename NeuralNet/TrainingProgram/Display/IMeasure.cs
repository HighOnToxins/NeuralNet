
namespace NeuralNet.TrainingProgram.Display;

public interface IMeasure
{
    public IEnumerable<string> CollectFields();

    public IEnumerable<object> CollectInfo(INet net);

}
