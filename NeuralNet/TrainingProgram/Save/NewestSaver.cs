
namespace NeuralNet.TrainingProgram.Save;

public sealed class NewestSaver: INetSaver
{

    private readonly string path;

    public NewestSaver(string path) 
    { 
        this.path = path;
    }

    public void Save(INet net)
    {
        net.Save(path);
    }
}
