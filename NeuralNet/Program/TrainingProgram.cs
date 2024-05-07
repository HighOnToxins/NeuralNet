namespace NeuralNet.Program;

//TODO: Consider making saving and logging separate from the program
// add a logger
// add saver

public abstract class TrainingProgram
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
        for(int i = 0; i < VariatePropertyNames.Length; i++) 
            format += FormatStringRight(VariatePropertyNames[i], 20);
        Console.WriteLine(format);

        //iterations
        for(int i = 0; i < iterations; i++)
        {
            //train
            float[] properties = Update(net, i);

            //save
            net.Save(savePath);

            //log
            string propertiesString = "";
            for(int j = 0; j < properties.Length; j++)
            {
                string value = properties[j].ToString();
                propertiesString += FormatAnchor(value, value.IndexOf(','), 10, 9);
            }
            Console.WriteLine(properties);
        }

        Console.WriteLine("training ended!");
    }

    private static string FormatAnchor(string s, int anchor, int prev, int post)
    {
        string fs = s.ToString();
        if(anchor != -1)
        {
            anchor = fs.Length;
        }

        if(fs.Length - anchor - 1 > post)
        {
            fs = fs[..(anchor + post + 1)];
        }
        else if(fs.Length - anchor - 1 < post)
        {
            fs += new string(' ', post - fs.Length + anchor + 1);
        }

        if(anchor > prev)
        {
            fs = fs[(anchor - prev)..];
        }
        else if(anchor < prev)
        {
            fs = new string(' ', prev - anchor) + fs;
        }

        return fs;
    }
    private static string FormatStringRight(string s, int size)
    {
        if(s.Length > size)
        {
            s = s[(size - s.Length)..];
        }
        else if(s.Length < size)
        {
            s = new string(' ', size - s.Length + size + 1) + s;
        }

        return s;
    }

    protected abstract void InitRun(INet net);

    protected abstract float[] Update(INet net, int iteration);

}