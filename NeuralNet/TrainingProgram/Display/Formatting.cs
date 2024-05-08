
namespace NeuralNet.TrainingProgram.Display;

internal static class Formatting
{
    public static string FormatRight(string s, int size, char fill = ' ')
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

    public static string FormatLeft(string s, int size, char fill = ' ')
    {
        if(s.Length > size)
        {
            s = s[..(s.Length - size)];
        }
        else if(s.Length < size)
        {
            s += new string(fill, size - s.Length);
        }

        return s;
    }

    public static string FormatAnchor(string s, int anchor, int prev, int post, char fill = ' ')
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

    public static string FormatCenter(string s, int size, char fill = ' ')
    {
        return FormatAnchor(s, s.Length / 2, (int)Math.Floor(size / 2f), (int)Math.Ceiling(size / 2f), fill);
    }

}
