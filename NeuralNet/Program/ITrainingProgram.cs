namespace NeuralNet.Program;

//TODO: Consider making saving and logging seperate from the program
// add a logger
// add saver

public abstract class ITrainingProgram
{

    protected abstract string[] VariatePropertyNames { get; }
    protected abstract string[] ConstantPropertyNames { get; }
    protected abstract float[] ConstantProperties { get; }

    public void Run(INet net, int iterations, string savePath)
    {
        Console.WriteLine("Started training!\n");

        InitRun(net);

        //print constant properties
        string[] constantPropertyNames = ConstantPropertyNames;
        float[] constantProperties = ConstantProperties;
        for(int i = 0; i < constantProperties.Length; i++)
        {
            Console.WriteLine(string.Format("{0} = {1}", constantPropertyNames[i], constantProperties[i]));
        }

        if(constantProperties.Length != 0)
        {
            Console.WriteLine();
        }

        //print properties
        string format = "";
        string[] variatePropertyNames = VariatePropertyNames;
        for(int i = 0; i < variatePropertyNames.Length; i++) format += $"{{{i}, 20}}\t";
        Console.WriteLine(string.Format(format, variatePropertyNames));

        for(int i = 0; i < iterations; i++)
        {
            //train
            float[] properties = Update(net, i);

            //save
            net.Save(savePath);

            //log
            Console.WriteLine(string.Format(format, properties));
        }

        net.Save(savePath);

        Console.WriteLine("saved net!");
        Console.WriteLine("training ended!");
    }

    protected abstract void InitRun(INet net);

    protected abstract float[] Update(INet net, int iteration);

}