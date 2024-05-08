
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
        for(int i = 0; i < fields.Length; i++)
        {
            Console.WriteLine(Formatting.FormatCenter(fields[i], boxSize, fill));
        }
    }

    public void Log(float[] info)
    {
        for(int i = 0; i < info.Length; i++)
        {
            string s = info[i].ToString();
            Console.WriteLine(Formatting.FormatAnchor(s, s.IndexOf(','), boxSize, fill));
        }
    }
}
