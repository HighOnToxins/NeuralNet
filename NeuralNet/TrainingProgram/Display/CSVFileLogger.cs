
namespace NeuralNet.TrainingProgram.Display;

public class CSVFileLogger: ILogger
{

    private readonly string path;
    private readonly char columnSep;
    private readonly char rowSep;

    public CSVFileLogger(string path, char columnSep = ';', char rowSep = '\n')
    {
        this.path = path;
        this.columnSep = columnSep;
        this.rowSep = rowSep;
    }

    public void InitLog(string[] fields)
    {
        using StreamWriter writer = new(File.Create(path));

        for(int i = 0; i < fields.Length; i++) 
        {
            writer.Write(fields[i] + columnSep);
        }

        writer.Write(rowSep);
    }

    public void Log(float[] info)
    {
        using StreamWriter writer = new(File.Create(path));

        for(int i = 0; i < info.Length; i++)
        {
            string s = info[i].ToString();
            writer.Write(s + columnSep);
        }

        writer.Write(rowSep);
    }
}
