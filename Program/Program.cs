
using NeuralNet;
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Program;

namespace Training;

internal class Program
{
    private const string mnistDirectory = "../../../MNISTfiles/";
    private const string netDirectory = "../../../net/";
    private const string netPath = netDirectory + "gradientDenseNet";

    private const int dataUse = 100;

    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(float[] output1, float[] output2)
        {
            float totalDifference = 0;
            for(int i = 0; i < output1.Length; i++)
            {
                totalDifference += Math.Abs(output2[i] - output1[i]);
            }
            return totalDifference;
        }

        public float[] Gradient(float[] output1, float[] output2)
        {
            float[] gradient = new float[output1.Length];
            for(int i = 0; i < output1.Length; i++)
            {
                gradient[i] = Math.Sign(output2[i] - output1[i]);
            }
            return gradient;
        }
    }

    private static float[][] AssignImageData(byte[][,] images)
    {
        float[][] values = new float[dataUse >= 0 ? dataUse : images.Length][];

        for(int i = 0; i <  values.Length; i++)
        {
            values[i] = new float[images[i].GetLength(0)*images[i].GetLength(1)];
            for(int j = 0; j < values[i].Length; j++)
            {
                values[i][j] = images[i][j % images[i].GetLength(0), j / images[i].GetLength(0)] / 255f;
            }
        }

        return values;
    }

    private static float[][] AssignLabelData(byte[] labels)
    {
        float[][] values = new float[dataUse >= 0 ? dataUse : labels.Length][];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = new float[10];
            values[i][ labels[i] ] = 1;
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
        float[][] trainingTargetData = AssignLabelData(trainLabels);
        float[][] trainingInputData  = AssignImageData(trainImages);
        float[][] testingTargetData  = AssignLabelData(testLabels);
        float[][] testingInputData   = AssignImageData(testImages);

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
        MomentumProgram program = new(trainer, 1f / trainingInputData.Length, .5f);
        program.Run(net, 100, netPath);
    }

}
