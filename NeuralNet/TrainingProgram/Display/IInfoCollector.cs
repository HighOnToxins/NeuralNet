
namespace NeuralNet.TrainingProgram.Display;

public interface IInfoCollector
{
    public IEnumerable<string> CollectFields();

    public IEnumerable<object> CollectInfo(INet net);

}
