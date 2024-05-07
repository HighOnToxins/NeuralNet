
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Tensor;

namespace NetTest;

internal class FileTests
{

    public const string path = "../../../savedAgents/";

    [SetUp]
    public void CreateSavedAgentsFolder()
    {
        Directory.CreateDirectory(path);
    }

    [Test]
    public void SaveLoadTest()
    {
        FeedforwardNet net = new(
            10,
            new AffineLayer(2, 10),
            new AffineLayer(10, 2)
        );

        net.Save(path + "saveTest1");
        Vector weights = net.GetWeights();

        net.SetWeights(new Vector(weights.Height));

        net.Load(path + "saveTest1");
        Vector weights2 = net.GetWeights();

        MatrixTests.AssertEquivalentVectors(weights, weights2);
    }
}
