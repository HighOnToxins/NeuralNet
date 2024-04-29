
using NeuralNet;
using NeuralNet.Feedforward.Layers;

namespace NetTest;

internal class LayerTests
{

    [Test]
    public void TestAffineLayerWeights()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9};
        AffineLayer layer = new(2, 3, weights);

        Assert.That(layer.GetWeightLength(), Is.EqualTo(9));

        Assert.That(layer.GetWeights(), Is.EquivalentTo(weights));

    }

    [Test]
    public void TestAffineLayerRun()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        AffineLayer layer = new(2, 3, weights);

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

    [Test]
    public void TestAffineLayerWeightGradient()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        AffineLayer layer = new(2, 3, weights);

        float[] input = new float[]
        {
            10, 11
        };

        float[,] expected = new float[,]
        {
            { 10, 11,  0,  0,  0,  0, 1, 0, 0},
            {  0,  0, 10, 11,  0,  0, 0, 1, 0},
            {  0,  0,  0,  0, 10, 11, 0, 0, 1},
        };

        float[,] result = layer.ComputeWeightGradient(input);

        MatrixTests.AssertEquivalentMatrices(result, expected);
    }

    [Test]
    public void TestAffineLayerInputGradient()
    {
        float[] weights = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        AffineLayer layer = new(2, 3, weights);

        float[] input = new float[]
        {
            10, 11
        };

        float[,] expected = new float[,]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 },
        };

        float[,] result = layer.ComputeInputGradient(input);

        MatrixTests.AssertEquivalentMatrices(result, expected);
    }

}
