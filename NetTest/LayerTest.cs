
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;

namespace NetTest;

internal class LayerTest
{
    private class IdentityFunction: IActivation
    {
        public float[,] ComputeGradient(float[] input)
        {
            float[,] result = new float[input.Length,input.Length];

            for(int i = 0; i < input.Length; i++)
            {
                for(int j = 0; j < input.Length; j++)
                {
                    result[i,j] = 1;
                }
            }

            return result;
        }

        public float[] Run(float[] input)
        {
            return input;
        }
    }

    [Test]
    public void TestAffineLayerWeights()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9};
        AffineLayer layer = new(null, 2, 3, weights);

        Assert.That(layer.GetWeightLength(), Is.EqualTo(9));

        Assert.That(layer.GetWeights(), Is.EquivalentTo(weights));

    }

    [Test]
    public void TestAffineLayerRun()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        AffineLayer layer = new(new IdentityFunction(), 2, 3, weights);

        float[] input = new float[] 
        { 
            10, 11
        };

        float[] expected = new float[]
        {
            39, 82, 125
        };

        float[] result = layer.Run(input);

        Assert.That(result, Is.EquivalentTo(expected));
    }

}
