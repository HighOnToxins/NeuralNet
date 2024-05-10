
namespace NeuralNet.TrainingProgram.Display;

public class CSVLogger: ILogger
{

    private readonly string path;
    private readonly char columnSep;
    private readonly char rowSep;

    public CSVLogger(string path, char columnSep = ';', char rowSep = '\n')
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

    public void Log(object[] info)
    {
        //TODO: Remove separators from information
        using StreamWriter writer = new(path, true);

        for(int i = 0; i < info.Length; i++)
        {
            if(info[i].ToString() is not string s)
            {
                throw new ArgumentException();
            }

            writer.Write(s + columnSep);
        }

        writer.Write(rowSep);
    }
}
