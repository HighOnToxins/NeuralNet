
using System.Drawing;
using System.Runtime.InteropServices;

namespace Program;

internal class Start
{
    const string directory = "../../../MNISTfiles/";

    public static void Main(string[] _)
    {
        int[] trainLabels = MNISTLoader.LoadLabels(directory, MNISTLoader.LoadType.trainingData);
        int[][] trainData = MNISTLoader.LoadImages(directory, MNISTLoader.LoadType.trainingData);
        int[] testLabels = MNISTLoader.LoadLabels(directory, MNISTLoader.LoadType.testingData);
        int[][] testData = MNISTLoader.LoadImages(directory, MNISTLoader.LoadType.testingData);

        // TODO: add training run for MNIST network



    }
}
