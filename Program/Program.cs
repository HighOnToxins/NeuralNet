
using NeuralNet;
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Program;
using NeuralNet.Tensor;

namespace Training;

internal class Program
{
    private const string mnistDirectory = "../../../MNISTfiles/";
    private const string netDirectory = "../../../net/";
    private const string netPath = netDirectory + "gradientDenseNet";

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

    private static Vector[] AssignImageData(byte[][,] images)
    {
        Vector[] values = new Vector[dataUse >= 0 ? dataUse : images.Length];

        for(int i = 0; i <  values.Length; i++)
        {
            float[] vecValues = new float[MNISTLoader.ImageSize* MNISTLoader.ImageSize];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = images[i][j % MNISTLoader.ImageSize, j / MNISTLoader.ImageSize] / 255f;
            }
            values[i] = new Vector(vecValues);
        }

        return values;
    }

    private static Vector[] AssignLabelData(byte[] labels)
    {
        Vector[] values = new Vector[dataUse >= 0 ? dataUse : labels.Length];

        for(int i = 0; i < values.Length; i++)
        {
            float[] vecValues = new float[10];
            vecValues[ labels[i] ] = 1;
            values[i] = new(vecValues);
        }

        return values;
    }

    public static void Main(string[] _)
    {
        //getting data
        byte[]    trainLabels = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.trainingData);
        byte[][,] trainImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.trainingData);
        byte[]    testLabels  = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.testingData );
        byte[][,] testImages  = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.testingData );

        //converting data into a usable form
        Vector[] trainingTargetData = AssignLabelData(trainLabels);
        Vector[] trainingInputData  = AssignImageData(trainImages);
        Vector[] testingTargetData  = AssignLabelData(testLabels);
        Vector[] testingInputData   = AssignImageData(testImages);

        Console.WriteLine("Loaded files!");

        //setting up trainer and network
        FeedForwardTrainer trainer = new(trainingInputData, trainingTargetData, new LossFunction());

        FeedforwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU(.05f))
        );

        if(!File.Exists(netPath + ".bin"))
        {
            Directory.CreateDirectory(netDirectory);
            net.Randomize(x => 500f * (float) Math.Pow(x - .5f, 7));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        //running program
        MomentumProgram program = new(trainer, .1f / trainingInputData.Length, 0);
        //NewtonProgram program = new(trainer);
        program.Run(net, 100, netPath);
    }

}
