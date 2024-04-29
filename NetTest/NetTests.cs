
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using static NetTest.LayerTests;

namespace NetTest;

internal class NetTests
{

    [Test]
    public void TestFeedforwardInit()
    {
        float[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        float[] weights2 = { 10, 11, 12, 13 };

        FeedForwardNet net = new(
            new AffineLayer(new IdentityFunction(), 2, 3, weights1),
            new AffineLayer(new IdentityFunction(), 3, 1, weights2)
        );

        Assert.That(net.GetWeightLength(), Is.EqualTo(13));

        Assert.That(net.InputSize, Is.EqualTo(2));
        Assert.That(net.OutputSize, Is.EqualTo(1));

        float[] expectedWeights = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

        Assert.That(net.GetWeights(), Is.EquivalentTo(expectedWeights));

    }

    [Test]
    public void TestFeedforwardRun()
    {
        float[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        float[] weights2 = { 10, 11, 12, 13 };

        FeedForwardNet net = new(
            new AffineLayer(new IdentityFunction(), 2, 3, weights1),
            new AffineLayer(new IdentityFunction(), 3, 1, weights2)
        );

        float[] input = { 14, 15 };
        float[] expected = { 3761 };

        float[] result = net.Run(input);

        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void TestFeedforwardGradient()
    {
        float[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        float[] weights2 = { 10, 11, 12, 13 };

        FeedForwardNet net = new(
            new AffineLayer(new IdentityFunction(), 2, 3, weights1),
            new AffineLayer(new IdentityFunction(), 3, 1, weights2)
        );

        float[] input = { 14, 15 };
        float[] expectedResult = { 3761 };
        float[,] expectedGradient = {
            { 51, 110, 169,   1,   0,   0,  0,  0,  0, 0, 0, 0, 0 },
            {  0, 140, 150, 154, 165, 168, 180, 10, 11, 12, 0, 0, 0 }
        };

        (float[,] gradient, float[] result) = net.ComputeGradient(input);

        Assert.That(result, Is.EquivalentTo(expectedResult));
        MatrixTests.AssertEquivalentMatrices(gradient, expectedGradient);
    }
}
