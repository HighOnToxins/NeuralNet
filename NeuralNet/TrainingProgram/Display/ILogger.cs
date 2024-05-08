
namespace NeuralNet.TrainingProgram.Display;

public interface ILogger
{
    public void InitLog(string[] fields);

    public void Log(float[] info);

}
