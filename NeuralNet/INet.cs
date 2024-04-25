
namespace NeuralNet;

public interface INet: ILayer
{

    public (float[,], float[]) ComputeGradient(float[] input);

}
