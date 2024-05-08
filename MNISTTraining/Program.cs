
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.TrainingProgram;
using NeuralNet.Tensor;
using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;

namespace MNSITTraining;

internal class Program
{
    private const string projectFolder = "../../../";

    private const string MNISTDirectory = projectFolder + "MNISTFiles/";
    
    private const string netDirectory = projectFolder + "net/";

    private const string logDirectory = projectFolder + "logs/";

    private const int dataUse = 1_000;

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

    private static Vector[] AssignTrainingLabels(byte[] labels, int categoryCount)
    {
        int maxUse = dataUse >= 0 ? dataUse : labels.Length;

        Vector[] data = new Vector[maxUse];

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[categoryCount];
            vecValues[labels[i]] = 1;
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

    private static Vector[] AssignTestingLabels(int categoryCount)
    {
        Vector[] result = new Vector[categoryCount];

        for(int i = 0; i < categoryCount; i++)
        {
            float[] floats = new float[categoryCount];
            floats[i] = 1;
            result[i] = new(floats);
        }

        return result;
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
        byte[][,] trainImages = MNISTLoader.LoadImages(MNISTDirectory, MNISTLoader.LoadType.trainingData);
        byte[]    trainLabels = MNISTLoader.LoadLabels(MNISTDirectory, MNISTLoader.LoadType.trainingData);

        byte[][,] testImages = MNISTLoader.LoadImages(MNISTDirectory, MNISTLoader.LoadType.testingData);
        byte[]    testLabels  = MNISTLoader.LoadLabels(MNISTDirectory, MNISTLoader.LoadType.testingData );

        //converting data into a usable form
        Vector[] trainingInputData  = AssignTrainingData(trainImages);
        Vector[] trainingTargets = AssignTrainingLabels(trainLabels, MNISTLoader.CategoryCount);

        Vector[][] testingInputData   = AssignTestingData(testImages, testLabels, MNISTLoader.CategoryCount);
        Vector[] testingLabels = AssignTestingLabels(MNISTLoader.CategoryCount);

        Console.WriteLine("Loaded files!");

        //setting up log folder
        if(!Directory.Exists(logDirectory)) 
            Directory.CreateDirectory(logDirectory);

        //trainer, tester & program
        LossFunction loss = new();

        FeedForwardTrainer trainer = new(trainingInputData, trainingTargets, loss);
        CategoryTester tester = new(testingInputData, testingLabels, loss, Guess, MNISTLoader.CategoryCount);
        ConstantRateProgram program = new(trainer);
        //MomentumProgram program = new(trainer, dataUse/1e3f, 0);

        //runner 
        string now = DateTime.Now.ToString().Replace('/', '_').Replace('.', '_');
        string netPath = netDirectory + now;
        TrainingRunner runner = new(program,
            new IMeasure[] 
            { 
                new IterationMeasure(), 
                new TimeMeasure(),
                new LossMeasure( trainer, tester )
            },
            new ILogger[] 
            { 
                new ConsoleLogger(), 
                new CSVFileLogger(logDirectory + now + ".csv"),
            },
            new NewestSaver(netPath)
         );

        //network
        FeedforwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize * MNISTLoader.ImageSize, MNISTLoader.CategoryCount, new ReLU(.05f))
        );

        if(!Directory.Exists(netDirectory))
            Directory.CreateDirectory(netDirectory);

        if(!File.Exists(netPath + ".bin"))
        {
            net.Randomize(x => 100f * (float)Math.Pow(x - .5f, 7));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        Console.WriteLine("Started training!\n");
        runner.Run(net, 100);
        Console.WriteLine("\nTraining ended!\n");

        Matrix confusionMatrix = tester.ConfusionMatrix(net);
        Console.WriteLine($"\n\nCONFUSION MATRIX:\n{confusionMatrix.ToString()}");
    }
}
