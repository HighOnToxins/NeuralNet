
namespace NeuralNet.TrainingProgram;

public enum TrainingOption
{
    Maximize = 1,
    Minimize = -1
}

public interface ITrainingProgram
{
    public void InitRun(INet net);

    public void Update(INet net);
}
