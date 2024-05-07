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
            format += FormatCenter(VariatePropertyNames[i], 20);
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
                propertiesString += FormatAnchor(value, value.IndexOf(','), 10, 10);
            }
            Console.WriteLine(propertiesString);
        }

        Console.WriteLine("training ended!");
    }

    private static string FormatAnchor(string s, int anchor, int prev, int post, char fill = ' ')
    {
        string fs = s.ToString();
        if(anchor == -1)
        {
            anchor = fs.Length;
        }

        if(fs.Length - anchor > post)
        {
            fs = fs[..(anchor + post)];
        }
        else if(fs.Length - anchor < post)
        {
            fs += new string(fill, post - fs.Length + anchor);
        }

        if(anchor > prev)
        {
            fs = fs[(anchor - prev)..];
        }
        else if(anchor < prev)
        {
            fs = new string(fill, prev - anchor) + fs;
        }

        return fs;
    }
    
    private static string FormatRight(string s, int size, char fill = ' ')
    {
        if(s.Length > size)
        {
            s = s[(s.Length - size)..];
        }
        else if(s.Length < size)
        {
            s = new string(fill, size - s.Length) + s;
        }

        return s;
    }

    private static string FormatCenter(string s, int size, char fill = ' ')
    {
        return FormatAnchor(s, s.Length / 2, (int)Math.Floor(size/2f), (int)Math.Ceiling(size/2f), fill);
    }

    protected abstract void InitRun(INet net);

    protected abstract float[] Update(INet net, int iteration);

}