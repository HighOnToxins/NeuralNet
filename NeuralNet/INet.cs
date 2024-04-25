
namespace NeuralNet;

public interface INet: ILayer
{

    public float[,] ComputeGradient(float[] input);

}
