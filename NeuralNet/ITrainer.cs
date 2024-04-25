
namespace NeuralNet;

internal interface ITrainer
{

    public INet Net { get; }

    public void Train(int iterations = 1);

    public float Loss();

}
