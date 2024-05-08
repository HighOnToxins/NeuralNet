using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram.Testing;

public sealed class CategoryTester : ITester
{
    private readonly Vector[][] testingData;

    private readonly Vector[] labels;

    private readonly ILoss loss;

    public CategoryTester(Vector[][] testingData, Vector[] labels, ILoss loss)
    {
        this.testingData = testingData;
        this.labels = labels;
        this.loss = loss;
    }

    public float Loss(INet net)
    {
        float totalLoss = 0;

        for (int i = 0; i < testingData.Length; i++)
        {
            for (int j = 0; j < testingData[i].Length; j++)
            {
                totalLoss += loss.Compute(labels[i], net.Run(testingData[i][j]));
            }
        }

        return totalLoss;
    }
}
