
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
    
    private const int dataUse = 1000;

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

    private static Vector MaxGuess(Vector output)
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

    private static Vector FullGuess(Vector output)
    {
        return output;
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
        CategoryTester tester = new(testingInputData, testingLabels, loss, MaxGuess, MNISTLoader.CategoryCount);

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

        layerCount = 8;
        layerBreadth = 5;
        activation = new ReLU(.05f);

        AddParameterNote(runDirectory, layerCount, layerBreadth, activation.GetType().Name);

        Stopwatch timer = new();
        timer.Start();
        
        for(int i = 0; i < 4; i++)
        {
            LinearRateTraining(runDirectory + "Gradient Descent (linear)/", trainer, tester, SciNot(-i - 5), iterationCount, i+1);
        }

        for(int i = 0; i < 4; i++)
        {
            NewtonsMethodTraining(runDirectory + "Newtons Method/", trainer, tester, SciNot(-i), iterationCount, i+1);
        }

        for(int i = 0; i < 4; i++)
        {
            ConstantRateTraining(runDirectory + "Gradient Descent (constant)/", trainer, tester, SciNot(-i), iterationCount, i + 1);
        }
        
        timer.Stop();
        Console.WriteLine($"COMPLETED FULL TRAINING! in {timer.Elapsed}");
        Console.WriteLine("program ended!");
    }

    private static float SciNot(int x)
    {
        return (float) Math.Pow(10, x);
    }

    private static int layerCount;
    private static int layerBreadth;
    private static IActivation? activation;

    private static void AddParameterNote(string runDirectory, int layerCount, int layerBreadth, string activationName)
    {
        if(!Directory.Exists(runDirectory))
            Directory.CreateDirectory(runDirectory);

        StreamWriter writer = new(runDirectory + "parameters.txt", false);
        writer.WriteLine($"layerCount = {layerCount}");
        writer.WriteLine($"layerBreadth = {layerBreadth}");
        writer.WriteLine($"activationName = {activationName}");
        writer.Close();
    }

    public static INet CreateMNISTNetwork()
    {
        IFeedForwardLayer[] layers = new IFeedForwardLayer[layerCount];
        
        if(layerCount == 1)
        {
            layers[0] = new AffineLayer(MNISTLoader.ImageSize * MNISTLoader.ImageSize, MNISTLoader.CategoryCount, activation);
        }
        else
        {
            layers[0] = new AffineLayer(MNISTLoader.ImageSize * MNISTLoader.ImageSize, layerBreadth, activation);
        }

        for(int i = 1; i < layerCount - 1; i++)
        {
            layers[i] = new AffineLayer(layerBreadth, layerBreadth, activation);
        }

        layers[^1] = new AffineLayer(layerBreadth, MNISTLoader.CategoryCount, activation);

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
        new FrequencySaver(netPath, doOverrideFile: false)
    );

    public static void SetupAndRun(ITrainingProgram program, ITrainer trainer, CategoryTester tester, string directory, int iterationCount, float learningRate, int fileNum)
    {
        INet? net = null;

        string fileName =
            $"({fileNum}) "
            + learningRate.ToString().Replace('.', '_')
            + "_" + layerCount
            + "_" + layerBreadth
            + "_" + activation?.GetType().Name;
        string netSaveDirectory = directory + $"Saved Nets/{fileNum}/";

        try
        {
            if(!Directory.Exists(netSaveDirectory))
                Directory.CreateDirectory(netSaveDirectory);

            string csvPath = directory + fileName;
            string netPath = netSaveDirectory + "net";

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
            net?.Save(netSaveDirectory + "caughtNet");

            StreamWriter writer = new(directory + $"({fileNum}) error message.txt", true);
            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine(e.ToString());
            writer.WriteLine(e.Message);
            writer.WriteLine(e.StackTrace);
            writer.WriteLine(e.Source);

            Console.WriteLine("\nPROGRAM CRASHED!\n");
        }
    }

    public static void LinearRateTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount, int num)
    {
        //screen logging
        Console.WriteLine(" - - - Linear rate training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        LinearRateProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount, learningRate, num);
    }

    public static void NewtonsMethodTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount, int num)
    {
        //screen logging
        Console.WriteLine(" - - - Newtons method training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        NewtonProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount, learningRate, num);
    }

    public static void ConstantRateTraining(string directory, ITrainer trainer, CategoryTester tester, float learningRate, int iterationCount, int num)
    {
        //screen logging
        Console.WriteLine(" - - - Constant rate training - - - ");
        Console.WriteLine($"learning rate = {learningRate}");

        //program
        ConstantRateProgram program = new(trainer, learningRate);

        SetupAndRun(program, trainer, tester, directory, iterationCount, learningRate, num);
    }

}
