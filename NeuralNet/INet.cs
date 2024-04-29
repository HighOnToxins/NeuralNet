
namespace NeuralNet;

public interface INet: ILayer
{

    public (float[,], float[]) ComputeGradient(float[] input);

    public void Save(string path);

    public void Load(string path);

}
