
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;

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
        FeedForwardNet net = new(
            10,
            new AffineLayer(2, 10),
            new AffineLayer(10, 2)
        );

        net.Save(path + "saveTest1");
        float[] weights = net.GetWeights();

        net.SetWeights(new float[weights.Length]);

        net.Load(path + "saveTest1");
        float[] weights2 = net.GetWeights();

        Assert.That(weights, Is.EquivalentTo(weights2));
    }
}
