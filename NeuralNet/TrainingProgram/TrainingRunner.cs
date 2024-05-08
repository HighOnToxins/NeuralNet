using NeuralNet.TrainingProgram.Display;
using NeuralNet.TrainingProgram.Save;

namespace NeuralNet.TrainingProgram;

/// <summary> Runs a given training program, logging data in the specified manner. </summary>
public sealed class TrainingRunner
{

    private readonly ITrainingProgram program;
    
    private readonly IInfoCollector[] collectors;

    private readonly ILogger[] loggers;

    private readonly INetSaver? saver;

    public TrainingRunner(ITrainingProgram program, IInfoCollector[] collectors, ILogger[] loggers, INetSaver? saver = null)
    {
        this.program = program;
        this.loggers = loggers;
        this.collectors = collectors;
        this.saver = saver;
    }

    public void Run(INet net, int iterations)
    {
        //init logs 
        InitLogs(CollectFields());

        //init program
        program.InitRun(net);

        //iterations
        for(int i = 0; i < iterations; i++)
        {
            //train
            program.Update(net);

            //possibly save net
            saver?.Save(net);

            //write to logs
            Log(CollectInfo(net));
        }
    }

    private void InitLogs(string[] fields)
    {
        for(int i = 0; i < loggers.Length; i++)
        {
            loggers[i].InitLog(fields);
        }
    }

    private void Log(object[] info)
    {
        for(int i = 0; i < loggers.Length; i++)
        {
            loggers[i].Log(info);
        }
    }

    private string[] CollectFields()
    {
        List<string> fields = new();
        for(int i = 0; i < collectors.Length; i++)
        {
            fields.AddRange(collectors[i].CollectFields());
        }
        return fields.ToArray();
    }

    private object[] CollectInfo(INet net)
    {
        List<object> info = new();
        for(int i = 0; i < collectors.Length; i++)
        {
            info.AddRange(collectors[i].CollectInfo(net));
        }
        return info.ToArray();
    }

}