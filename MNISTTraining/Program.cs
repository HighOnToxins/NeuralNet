
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.TrainingProgram;

using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;
using NeuralNet;

using System.Diagnostics;
using MNISTTraining;

namespace MNSITTraining;

internal class Program
{
    private const string projectFolder = "../../../";
    private const string solutionFolder = projectFolder + "../";
    private const string runsDirectory = solutionFolder + "ProjectRuns/";

    private const string MNISTDirectory = projectFolder + "MNISTFiles/";
    
    public static void Main(string[] _)
    {
        NNMNISTHandler handler = new(MNISTDirectory);

        //directory
        if(!Directory.Exists(runsDirectory)) Directory.CreateDirectory(runsDirectory);

        int runCount = 0;
        while(Directory.Exists(runsDirectory + $"run{runCount}/"))
        {
            runCount++;
        }

        int[] layerBreadths = { 5, 10, 15 };
        int[] layerCounts = { 8, 5, 3 };
        activation = new ReLU(.05f);

        //running
        for(int i = 0; i <  layerCounts.Length; i++)
        {
            for(int j = 0; j <  layerBreadths.Length; j++)
            {
                if(i == 0 && j == 0) continue; 

                layerCount = layerCounts[i];
                layerBreadth = layerBreadths[j];

                try
                {
                    Run(runCount, handler.Trainer, handler.Tester);
                }
                catch(Exception e)
                {
                    StreamWriter writer = new(runsDirectory + $"run{runCount}/error message.txt", false);
                    writer.WriteLine(e.ToString());
                    writer.Close();

                    Console.WriteLine();
                    Console.Error.WriteLine(e.ToString());

                    Console.WriteLine($"\nRUNS CRASHED!\n");
                    Console.WriteLine($"Of types: " +
                        $"lc:{layerCount}, " +
                        $"lb:{layerBreadth}, " +
                        $"ac:{activation?.GetType().Name}\n");
                }

                runCount++;
            }
        }

    }

    private static void Run(int runCount, ITrainer trainer, CategoryTester tester)
    {
        string runDirectory = runsDirectory + $"run{runCount}/";

        int iterationCount = 50;

        AddParameterNote(runDirectory, layerCount, layerBreadth, activation.GetType().Name);

        Stopwatch timer = new();
        timer.Start();

        for(int i = 0; i < 4; i++)
        {
            LinearRateTraining(runDirectory + "Gradient Descent (linear)/", trainer, tester, SciNot(-i - 5), iterationCount, i + 1);
        }

        for(int i = 0; i < 4; i++)
        {
            NewtonsMethodTraining(runDirectory + "Newtons Method/", trainer, tester, SciNot(-i), iterationCount, i + 1);
        }

        for(int i = 0; i < 4; i++)
        {
            ConstantRateTraining(runDirectory + "Gradient Descent (constant)/", trainer, tester, SciNot(-i+1), iterationCount, i + 1);
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
            new CSVLogger(csvPath + ".csv", floatComma: '.'),
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
            net = NNMNISTHandler.CreateMNISTNetwork(layerCount, layerBreadth, activation);
            Console.WriteLine("Created New Network!");

            //running
            Console.WriteLine("Started training!\n");
            runner.Run(net, iterationCount);
            Console.WriteLine("\nTraining ended!\n");
        }
        catch(Exception e) 
        {
            net?.Save(netSaveDirectory + "caughtNet");

            StreamWriter writer = new(directory + $"({fileNum}) error message.txt", false);
            writer.WriteLine(e.ToString());
            writer.Close();

            Console.WriteLine();
            Console.Error.WriteLine(e.ToString());

            Console.WriteLine($"\nRUN CRASHED!\n");
            Console.WriteLine($"Of type: " +
                $"lr:{learningRate}, " +
                $"lc:{layerCount}, " +
                $"lb:{layerBreadth}, " +
                $"ac:{activation?.GetType().Name}\n");
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
