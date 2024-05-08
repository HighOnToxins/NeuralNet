
namespace NeuralNet.TrainingProgram;

public interface ITrainingProgram
{
    public void InitRun(INet net);

    public void Update(INet net);
}
