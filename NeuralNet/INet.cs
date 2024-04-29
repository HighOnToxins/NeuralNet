
namespace NeuralNet;

public interface INet: ILayer
{

    public (float[,], float[]) Gradient(float[] input);

    public void Save(string path);

    public void Load(string path);

}
