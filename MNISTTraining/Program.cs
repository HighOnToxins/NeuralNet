﻿
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.TrainingProgram;
using NeuralNet.Tensor;
using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;
using NeuralNet.TrainingProgram.Testing;

namespace MNSITTraining;

internal class Program
{
    private const string projectFolder = "../../../";

    private const string MNISTDirectory = projectFolder + "MNISTFiles/";
    
    private const string netDirectory = projectFolder + "net/";
    private const string netPath = netDirectory + "gradientDescentNet";

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

        //network
        FeedforwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, MNISTLoader.CategoryCount, new ReLU(.05f))
        );

        if(!File.Exists(netPath + ".bin"))
        {
            if(!Directory.Exists(netDirectory))
                Directory.CreateDirectory(netDirectory);

            net.Randomize(x => 200f * (float) Math.Pow(x - .5f, 7));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        //setting up log folder
        if(!Directory.Exists(logDirectory)) 
            Directory.CreateDirectory(logDirectory);

        //trainer, evaluator & program
        LossFunction loss = new();

        FeedForwardTrainer trainer = new(trainingInputData, trainingTargets, loss);
        Evaluator evaluator = new(testingInputData, Guess, MNISTLoader.CategoryCount);
        ConstantRateProgram program = new(trainer);

        //runner 
        string now = DateTime.Now.ToString().Replace('/', '_').Replace('.', '_');
        TrainingRunner runner = new(program,
            new IMeasure[] 
            { 
                new IterationMeasure(), 
                new TimeMeasure(),
                new LossMeasure(
                    new FlatDataTester(trainingInputData, trainingTargets, loss), 
                    new CategoryTester(testingInputData, testingLabels, loss))
            },
            new ILogger[] 
            { 
                new ConsoleLogger(), 
                new CSVFileLogger(logDirectory + now + ".csv"),
            },
            new NewestSaver(netPath)
         );

        Console.WriteLine("Started training!\n");
        runner.Run(net, 100);
        Console.WriteLine("Training ended!\n");

        Matrix confusionMatrix = evaluator.ConfusionMatrix(net);
        Console.WriteLine($"\n\nCONFUSION MATRIX:\n{confusionMatrix.ToString()}");
    }
}
