
namespace NeuralNet;

public interface ILayer
{

    public float[] Run(float[] input);

    public float[] GetWeights();

    public int GetWeightLength();

    void SetWeights(float[] newWeights);

    public void AddWeights(float[] newWeights);

}
