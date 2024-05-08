
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Program;
using NeuralNet.Tensor;

namespace Training;

internal class Program
{
    private const string mnistDirectory = "../../../MNISTfiles/";
    private const string netDirectory = "../../../net/";
    private const string netPath = netDirectory + "gradientDescentNet";

    private const int dataUse = 100;

    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(Vector output1, Vector output2)
        {
            float totalDifference = 0;
            for(int i = 0; i < output1.Height; i++)
            {
                totalDifference += Math.Abs(output2[i] - output1[i]);
            }
            return totalDifference;
        }

        public Vector Gradient(Vector output1, Vector output2)
        {
            float[] gradient = new float[output1.Height];
            for(int i = 0; i < output1.Height; i++)
            {
                gradient[i] = Math.Sign(output2[i] - output1[i]);
            }
            return new Vector(gradient);
        }
    }

    private static Vector[][] AssignData(byte[][,] images, byte[] labels, int categoryCount)
    {
        int maxUse = dataUse >= 0 ? dataUse : images.Length;

        List<Vector>[] inputList = new List<Vector>[categoryCount];
        for(int i = 0; i < categoryCount; i++)
        {
            inputList[i] = new();
        }

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[MNISTLoader.ImageSize * MNISTLoader.ImageSize];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = images[i][j % MNISTLoader.ImageSize, j / MNISTLoader.ImageSize] / 255f;
            }
            inputList[ labels[i] ].Add(new Vector(vecValues));
        }

        Vector[][] inputs = new Vector[categoryCount][];

        for(int i = 0; i < categoryCount; i++)
        {
            inputs[i] = inputList[i].ToArray();
        }

        return inputs;
    }

    private static Vector[] GenerateOneHotTargets(int categoryCount)
    {
        Vector[] targets = new Vector[categoryCount];

        for(int i = 0; i < targets.Length; i++)
        {
            float[] values = new float[categoryCount];
            values[i] = 1;
            targets[i] = new Vector(values);
        }
        return targets;
    }

    public static void Main(string[] _)
    {
        //getting data
        byte[][,] trainImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.trainingData);
        byte[]    trainLabels = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.trainingData);

        byte[][,] testImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.testingData);
        byte[]    testLabels  = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.testingData );

        //converting data into a usable form
        Vector[][] trainingInputData  = AssignData(trainImages, trainLabels, 10);
        Vector[][] testingInputData   = AssignData(testImages, testLabels, 10);
        Vector[] targets              = GenerateOneHotTargets(10);

        Console.WriteLine("Loaded files!");

        //setting up trainer and network
        FFCategoryTrainer trainer = new(trainingInputData, testingInputData, targets, new LossFunction());

        FeedforwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU(.05f))
        );

        if(!File.Exists(netPath + ".bin"))
        {
            Directory.CreateDirectory(netDirectory);
            net.Randomize(x => 200f * (float) Math.Pow(x - .5f, 7));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        //running program
        GradientDescentProgram program = new(trainer, v => v * (1f / v.Length()) );
        //MomentumProgram program = new(trainer, .025f / trainingInputData.Length, 0);
        //NewtonProgram program = new(trainer);
        program.Run(net, 100, netPath);

        Matrix confusionMatrix = trainer.ConfusionMatrix(net);
        Console.WriteLine($"\n\nCONFUSION MATRIX:\n{confusionMatrix.ToString()}");
    }
}
