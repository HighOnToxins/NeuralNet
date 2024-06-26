﻿
namespace NeuralNet.TrainingProgram.Display;

public class CSVLogger: ILogger
{

    private readonly string path;
    private readonly char columnSep;
    private readonly char rowSep;
    private readonly char floatComma;

    public CSVLogger(string path, char columnSep = ';', char rowSep = '\n', char floatComma = ',')
    {
        this.path = path;
        this.columnSep = columnSep;
        this.rowSep = rowSep;
        this.floatComma = floatComma;
    }

    public void InitLog(string[] fields)
    {
        using StreamWriter writer = new(File.Create(path));

        for(int i = 0; i < fields.Length; i++) 
        {
            writer.Write(fields[i]);

            if(i != fields.Length - 1)
            {
                writer.Write(columnSep);
            }
        }
    }

    public void Log(object[] info)
    {
        using StreamWriter writer = new(path, true);

        writer.Write(rowSep);

        for(int i = 0; i < info.Length; i++)
        {
            if(info[i].ToString() is not string s)
            {
                throw new ArgumentException();
            }

            if(info[i] is float)
            {
                s = s.Replace(',', floatComma);
            }

            writer.Write(s);

            if(i != info.Length - 1)
            {
                writer.Write(columnSep);
            }
        }
    }
}
