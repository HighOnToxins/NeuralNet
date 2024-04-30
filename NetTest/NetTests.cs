
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;

namespace NetTest;

internal class NetTests
{

    [Test]
    public void TestFeedforwardInit()
    {
        float[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        float[] weights2 = { 10, 11, 12, 13 };

        FeedForwardNet net = new(
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
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
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
        );

        float[] input = { 14, 15 };
        float[] expected = { 3761 };

        float[] result = net.Run(input);

        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void TestFeedforwardRun2()
    {
        FeedForwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        float[] input = { 14, 15 };
        net.Run(input);
    }

    [Test]
    public void TestFeedforwardRun3()
    {
        FeedForwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        float[] input = { 14, 15 };
        float[] result = net.Run(input);
        (float[,] _, float[] result2) = net.Gradient(input);

        Assert.That(result, Is.EquivalentTo(result2));
    }

    [Test]
    public void TestFeedforwardGradient()
    {
        float[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        float[] weights2 = { 10, 11, 12, 13 };

        FeedForwardNet net = new(
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
        );

        float[] input = { 14, 15 };
        float[] expectedResult = { 3761 };
        float[,] expectedGradient = {
            { 51, 140, 150, 154, 165, 168, 180, 10, 11, 12, 0, 0, 0 }
        };

        (float[,] gradient, float[] result) = net.Gradient(input);

        Assert.That(result, Is.EquivalentTo(expectedResult));
        MatrixTests.AssertEquivalentMatrices(gradient, expectedGradient);
    }

    [Test]
    public void TestFeedforwardGradient2()
    {
        float[] weights = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        AffineLayer layer = new(2, 3, weights);
        FeedForwardNet net = new(layer);

        float[] input = { 14, 15 };

        (float[,] gradient, _) = net.Gradient(input);
        (float[,] gradient2, _, _) = layer.Gradient(input);

        MatrixTests.AssertEquivalentMatrices(gradient, gradient2);
    }

    [Test]
    public void TestFeedforwardGradient3()
    {
        FeedForwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        float[] input = { 14, 15 };
        (float[,] _, float[] _) = net.Gradient(input);
    }

}
