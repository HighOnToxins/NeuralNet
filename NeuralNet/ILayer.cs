
using NeuralNet.Tensor;

namespace NeuralNet;

public interface ILayer
{

    public int InputSize { get; }
    public int OutputSize { get; }

    public Vector Run(Vector input);

    public Vector GetWeights();

    public int GetWeightLength();

    void SetWeights(Vector newWeights);

    public void AddWeights(Vector newWeights);

}
