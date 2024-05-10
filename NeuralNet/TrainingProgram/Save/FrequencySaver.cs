
namespace NeuralNet.TrainingProgram.Save;

public sealed class FrequencySaver: INetSaver
{

    private readonly string path;
    private readonly int saveFrequency;
    private readonly bool doOverrideFile;

    private int iterationCount;

    public FrequencySaver(string path, int saveFrequency = 1,  bool doOverrideFile = true) 
    { 
        this.path = path;
        this.saveFrequency = saveFrequency;
        this.doOverrideFile = doOverrideFile;

        iterationCount = 0;
    }

    public void Save(INet net)
    {
        if(iterationCount % saveFrequency == 0)
        {
            if(doOverrideFile)
            {
                net.Save(path);
            }
            else
            {
                net.Save(path + $" ({iterationCount})");
            }
        }

        iterationCount++;
    }
}
