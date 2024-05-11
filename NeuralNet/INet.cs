
using NeuralNet.Tensor;

namespace NeuralNet;

public interface INet: ILayer
{
    public Matrix Gradient(Vector input, out Vector result);

    public void Save(string path);

    public void Load(string path);
    
    public INet Clone();

}
