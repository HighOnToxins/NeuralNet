
namespace NeuralNet.Feedforward;

public interface IFeedForwardLoss
{

    public float Compute(float[] output1, float[] output2);

    public float[] Gradient(float[] output1, float[] output2);

}
