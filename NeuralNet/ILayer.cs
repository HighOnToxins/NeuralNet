
namespace NeuralNet;

public interface ILayer
{

    public int InputSize { get; }
    public int OutputSize { get; }

    public float[] Run(float[] input);

    public float[] GetWeights();

    public int GetWeightLength();

    void SetWeights(float[] newWeights);

    public void AddWeights(float[] newWeights);

}
