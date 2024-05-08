
namespace NeuralNet.TrainingProgram.Display;

public interface IInfoCollector
{
    public IEnumerable<string> CollectFields();

    public IEnumerable<float> CollectInfo(INet net);

}
