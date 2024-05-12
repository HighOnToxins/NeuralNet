
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.TrainingProgram;
using NeuralNet.Tensor;
using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;
using NeuralNet;
using System.Diagnostics.Metrics;
using System.IO;
using System.Diagnostics;

namespace MNSITTraining;

internal class Program
{
    private const string projectFolder = "../../../";
    private const string solutionFolder = projectFolder + "../";
    private const string runsDirectory = solutionFolder + "ProjectRuns/";

    private const string MNISTDirectory = projectFolder + "MNISTFiles/";
    
    private const int dataUse = -1;

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

        //trainer, tester & program
        LossFunction loss = new();

        FeedForwardTrainer trainer = new(trainingInputData, trainingTargets, loss);
        CategoryTester tester = new(testingInputData, testingLabels, loss, Guess, MNISTLoader.CategoryCount);

        //directory
        if(!Directory.Exists(runsDirectory)) Directory.CreateDirectory(runsDirectory);

        int runCount = 0;
        while(Directory.Exists(runsDirectory + $"run{runCount}/"))
        {
            runCount++;
        }
        string runDirectory = runsDirectory + $"run{runCount}/";

        //running
        int iterationCount = 50;

        layerCount = 3;
        layerBreath = 10;
        activation = new ReLU(.05f);

        Stopwatch timer = new();
        timer.Start();

        LinearRateTraining(runDirectory + "LR/_01/", trainer, tester, .01f, iterationCount);
        LinearRateTraining(runDirectory + "LR/_001/", trainer, tester, .001f, iterationCount);
        LinearRateTraining(runDirectory + "LR/_0001/", trainer, tester, .0001f, iterationCount);

        NewtonsMethodTraining(runDirectory + "NM/1/", trainer, tester, 1f, iterationCount);
        NewtonsMethodTraining(runDirectory + "NM/_01/", trainer, tester, .01f, iterationCount);
        NewtonsMethodTraining(runDirectory + "NM/_0001/", trainer, tester, .0001f, iterationCount);

        ConstantRateTraining(runDirectory + "CR/1/", trainer, tester, 1f, iterationCount);
        ConstantRateTraining(runDirectory + "CR/_1/", trainer, tester, .1f, iterationCount);
        ConstantRateTraining(runDirectory + "CR/_01/", trainer, tester, .01f, iterationCount);

        timer.Stop();
        Console.WriteLine($"COMPLETED FULL TRAINING! in {timer.Elapsed}");
        Console.WriteLine("program ended!");
    }

    private static int layerCount;
    private static int layerBreath;
    private static IActivation? activation;

    public static INet CreateMNISTNetwork()
    {
        IFeedForwardLayer[] layers = new IFeedForwardLayer[layerCount];
        
        if(layerCount == 0)
        {
            layers[0] = new AffineLayer(MNISTLoader.ImageSize * MNISTLoader.ImageSize, MNISTLoader.CategoryCount, activation);
        }
        else
        {
            layers[0] = new AffineLayer(MNISTLoader.ImageSize * MNISTLoader.ImageSize, layerBreath, activation);
        }

        for(int i = 1; i < layerCount - 1; i++)
        {
            layers[i] = new AffineLayer(layerBreath, layerBreath, activation);
        }

        layers[^1] = new AffineLayer(layerBreath, MNISTLoader.CategoryCount, activation);

        FeedforwardNet net = new(layers);
        net.Randomize(x => 100f * (float)Math.Pow(x - .5f, 7));

        return net;
    }

    public static TrainingRunner CreateRunner(ITrainingProgram program, ITrainer trainer, CategoryTester tester, string csvPath, string netPath) => new(program,
        new IMeasure[]
        {
            new IterationMeasure(),
            new TimeMeasure(),
            new LossMeasure( trainer, tester ),
            new EvaluationMeasure( tester, true, true )
        },
        new ILogger[]
        {
            new ConsoleLogger(),
            new CSVLogger(csvPath + ".csv"),
        },
        new FrequencySaver(netPath, 10, false)
    );

    public static void SetupAndRun(ITrainingProgram program, ITrainer trainer, CategoryTester tester, string directory, int iterationCount)
    {
        INet? net = null;

        try
        {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string csvPath = directory + "log";
            string netPath = directory + "net";

            //runner 
            TrainingRunner runner = CreateRunner(program, trainer, tester, csvPath, netPath);

            //network
            net = CreateMNISTNetwork();
            Console.WriteLine("Created New Network!");

            //running
            Console.WriteLine("Started training!\n");
            runner.Run(net, iterationCount);
            Console.WriteLine("\nTraining ended!\n");
        }
        catch(Exception e) 
        {
            net?.Save(runsDirectory + "caughtNet");

            StreamWriter writer = new(runsDirectory + "error message.txt", true);
            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine(e.ToString());
            writer.WriteLine(e.Message);
            writer.WriteLine(e.StackTrace);
            writer.WriteLine(e.Source);

            Console.WriteLine("\nPROGRAM CRASHED!\n");
        }
    }

    public static void LinearRateTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount)
    {
        //screen logging
        Console.WriteLine(" - - - Linear rate training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        MomentumProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount);
    }

    public static void NewtonsMethodTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount)
    {
        //screen logging
        Console.WriteLine(" - - - Newtons method training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        NewtonProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount);
    }

    public static void ConstantRateTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount)
    {
        //screen logging
        Console.WriteLine(" - - - Constant rate training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        ConstantRateProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount);
    }

}
