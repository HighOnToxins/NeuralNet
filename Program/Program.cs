
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.TrainingProgram;
using NeuralNet.Tensor;
using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;

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

    private static Vector[] AssignTrainingData(byte[][,] images)
    {
        int maxUse = dataUse >= 0 ? dataUse : images.Length;

        Vector[] data = new Vector[maxUse];

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[MNISTLoader.ImageSize * MNISTLoader.ImageSize];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = images[i][j % MNISTLoader.ImageSize, j / MNISTLoader.ImageSize] / 255f;
            }
            data[i] = new(vecValues);
        }

        return data;
    }

    private static Vector[] AssignTrainingLabels(byte[][] labels, int categoryCount)
    {
        int maxUse = dataUse >= 0 ? dataUse : labels.Length;

        Vector[] data = new Vector[maxUse];

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[categoryCount];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = labels[i][j] / 255f;
            }
            data[i] = new(vecValues);
        }

        return data;
    }

    private static Vector[][] AssignTestingData(byte[][,] images, byte[] labels, int categoryCount)
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

    private static Vector Guess(Vector output)
    {
        int maxIndex = 0;
        for(int i = 1; i < output.Height; i++)
        {
            if(output[i] > maxIndex)
            {
                maxIndex = i;
            }
        }

        float[] result = new float[MNISTLoader.CategoryCount];
        result[maxIndex] = 1;
        return new(result);
    }

    public static void Main(string[] _)
    {
        //getting data
        byte[][,] trainImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.trainingData);
        byte[]    trainLabels = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.trainingData);

        byte[][,] testImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.testingData);
        byte[]    testLabels  = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.testingData );

        //converting data into a usable form
        Vector[] trainingInputData  = AssignTrainingData(trainImages);
        Vector[] trainingTargets = AssignTrainingLabels(trainLabels, MNISTLoader.CategoryCount);

        Vector[][] testingInputData   = AssignTestingData(testImages, testLabels, MNISTLoader.CategoryCount);

        Console.WriteLine("Loaded files!");

        //network
        FeedforwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, MNISTLoader.CategoryCount, new ReLU(.05f))
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

        //trainer 
        FeedForwardTrainer trainer = new(trainingInputData, trainingTargets, new LossFunction());

        //evaluator
        Evaluator evaluator = new(testingInputData, Guess, MNISTLoader.CategoryCount);

        //program
        GradientDescentProgram program = new(trainer, v => v * (1f / v.Length()) );
        //MomentumProgram program = new(trainer, .025f / trainingInputData.Length, 0);
        //NewtonProgram program = new(trainer);

        //runner 
        string now = DateTime.Now.ToString().Replace('/', '_').Replace('.', '_');
        TrainingRunner runner = new(program,
            new IInfoCollector[] { new TimeCollector(), new IterationCollector() },
            new ILogger[] { new ConsoleLogger(), new CSVFileLogger("../../../net/logs/" + now + ".csv") },
            new NewestSaver(netPath)
         );

        Console.WriteLine("Started training!\n");
        runner.Run(net, 100);
        Console.WriteLine("Training ended!\n");

        Matrix confusionMatrix = evaluator.ConfusionMatrix(net);
        Console.WriteLine($"\n\nCONFUSION MATRIX:\n{confusionMatrix.ToString()}");
    }

    private static Vector[] AssignTrainingLabels(byte[] trainLabels, object categoryCount)
    {
        throw new NotImplementedException();
    }
}
