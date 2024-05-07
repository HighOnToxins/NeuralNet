
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Tensor;

namespace NetTest;

internal class NetTests
{

    [Test]
    public void TestFeedforwardInit()
    {
        Vector weights1 = new(new float[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        Vector weights2 = new(new float[]{ 10, 11, 12, 13 });

        FeedforwardNet net = new(
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
        );

        Assert.That(net.GetWeightLength(), Is.EqualTo(13));

        Assert.That(net.InputSize, Is.EqualTo(2));
        Assert.That(net.OutputSize, Is.EqualTo(1));

        Vector expectedWeights = new(new float[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 });
        MatrixTests.AssertEquivalentVectors(net.GetWeights(), expectedWeights);

    }

    [Test]
    public void TestFeedforwardRun()
    {
        Vector weights1 = new(new float[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        Vector weights2 = new(new float[]{ 10, 11, 12, 13 });

        FeedforwardNet net = new(
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
        );

        Vector input = new(new float[] { 14, 15 });
        Vector expected = new(new float[] { 3761 });

        Vector result = net.Run(input);

        MatrixTests.AssertEquivalentVectors(result, expected);
    }

    [Test]
    public void TestFeedforwardRun2()
    {
        FeedforwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        Vector input = new(new float[]{ 14, 15 });
        net.Run(input);
    }

    [Test]
    public void TestFeedforwardRun3()
    {
        FeedforwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        Vector input = new(new float[] { 14, 15 });
        Vector result = net.Run(input);
        (_, Vector result2) = net.Gradient(input);

        MatrixTests.AssertEquivalentVectors(result, result2);
    }

    [Test]
    public void TestFeedforwardGradient()
    {
        Vector weights1 = new(new float[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        Vector weights2 = new(new float[] { 10, 11, 12, 13 });

        FeedforwardNet net = new(
            new AffineLayer(2, 3, weights1),
            new AffineLayer(3, 1, weights2)
        );

        Vector input = new(new float[] { 14, 15 });
        Vector expectedResult = new(new float[] { 3761 });
        Matrix expectedGradient = new(new float[,]{
            { 51, 110, 169, 1, 140, 150, 154, 165, 168, 180, 10, 11, 12 }
        });

        (Matrix gradient, Vector result) = net.Gradient(input);

        MatrixTests.AssertEquivalentVectors(result, expectedResult);
        MatrixTests.AssertEquivalentMatrices(gradient, expectedGradient);
    }

    [Test]
    public void TestFeedforwardGradient2()
    {
        Vector weights = new(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        AffineLayer layer = new(2, 3, weights);
        FeedforwardNet net = new(layer);

        Vector input = new(new float[] { 14, 15 });

        (Matrix gradient, _) = net.Gradient(input);
        (Matrix gradient2, _, _) = layer.Gradient(input);

        MatrixTests.AssertEquivalentMatrices(gradient, gradient2);
    }

    [Test]
    public void TestFeedforwardGradient3()
    {
        FeedforwardNet net = new(
            new AffineLayer(2, 3),
            new AffineLayer(3, 5),
            new AffineLayer(5, 3),
            new AffineLayer(3, 2)
        );

        net.Randomize(10f);

        Vector input = new(new float[] { 14, 15 });
        (Matrix _, Vector _) = net.Gradient(input);
    }

}
