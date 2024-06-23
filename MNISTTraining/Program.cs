
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

        //running
        Run(runCount, handler.Trainer, handler.Tester);

    }

    private static void Run(int runCount, ITrainer trainer, CategoryTester tester)
    {
        string runDirectory = runsDirectory + $"run{runCount}/";

        int iterationCount = 20;

        //AddParameterNote(runDirectory, layerCount, layerBreadth, activation.GetType().Name);

        Stopwatch timer = new();
        timer.Start();

        LinearRateTraining(runDirectory + "Gradient Descent (linear)/", trainer, tester, 1e-6f, iterationCount, 0);
        NewtonsMethodTraining(runDirectory + "Newtons Method/", trainer, tester, 1e-2f, iterationCount, 0);

        timer.Stop();
        Console.WriteLine($"COMPLETED FULL TRAINING! in {timer.Elapsed}");
        Console.WriteLine("program ended!");
    }

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

        string fileName = "A";
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
            net = NNMNISTHandler.CreateMNISTNetwork();
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
