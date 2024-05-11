
using NeuralNet.Feedforward.Layers;
using NeuralNet.Tensor;

namespace NetTest;

internal class LayerTests
{

    [Test]
    public void TestAffineLayerWeights()
    {
        Vector weights = new(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9});
        AffineLayer layer = new(2, 3, weights);

        Assert.That(layer.GetWeightLength(), Is.EqualTo(9));

        MatrixTests.AssertEquivalentVectors(layer.GetWeights(), weights);
    }

    [Test]
    public void TestAffineLayerRun()
    {
        Vector weights = new(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        AffineLayer layer = new(2, 3, weights);

        Vector input = new(new float[] 
        { 
            10, 11
        });

        Vector expected = new(new float[]
        {
            39, 82, 125
        });

        Vector result = layer.Run(input);

        MatrixTests.AssertEquivalentVectors(result, expected);
    }

    [Test]
    public void TestAffineLayerWeightGradient()
    {
        Vector weights = new(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        AffineLayer layer = new(2, 3, weights);

        Vector input = new(new float[]
        {
            10, 11
        });

        Matrix expected = new(new float[,]
        {
            { 10, 11,  0,  0,  0,  0, 1, 0, 0},
            {  0,  0, 10, 11,  0,  0, 0, 1, 0},
            {  0,  0,  0,  0, 10, 11, 0, 0, 1},
        });

        Matrix result = layer.Gradient(input, out _, out _);

        MatrixTests.AssertEquivalentMatrices(result, expected);
    }

    [Test]
    public void TestAffineLayerInputGradient()
    {
        Vector weights = new(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        AffineLayer layer = new(2, 3, weights);

        Vector input = new(new float[]
        {
            10, 11
        });

        Matrix expected = new(new float[,]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 },
        });

        layer.Gradient(input, out Matrix result, out _);

        MatrixTests.AssertEquivalentMatrices(result, expected);
    }

}
