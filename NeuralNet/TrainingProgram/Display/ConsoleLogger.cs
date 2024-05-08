
namespace NeuralNet.TrainingProgram.Display;

public sealed class ConsoleLogger: ILogger
{

    private readonly int boxSize;
    private readonly char fill;

    public ConsoleLogger(int boxSize = 20, char fill = ' ') 
    {
        this.boxSize = boxSize;
        this.fill = fill;
    }


    public void InitLog(string[] fields)
    {
        Console.WriteLine();

        for(int i = 0; i < fields.Length; i++)
        {
            Console.Write(Formatting.FormatCenter(fields[i], boxSize, fill));
        }
    }

    public void Log(object[] info)
    {
        Console.WriteLine();

        string print = "";

        for(int i = 0; i < info.Length; i++)
        {
            if(info[i].ToString() is not string s)
            {
                throw new ArgumentException();
            }

            if(info[i] is float)
            {
                print += Formatting.FormatAnchorCenter(s, s.IndexOf(','), boxSize, fill);
            }
            else
            {
                print += Formatting.FormatCenter(s, boxSize, fill);
            }
        }

        Console.Write(print);
    }
}
