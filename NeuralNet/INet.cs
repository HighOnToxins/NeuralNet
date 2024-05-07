
using NeuralNet.Tensor;

namespace NeuralNet;

public interface INet: ILayer
{
    //TODO: change to record or struct
    // input -> gradient, results
    public (Matrix, Vector) Gradient(Vector input);

    public void Save(string path);

    public void Load(string path);
    
    public INet Clone();

}
