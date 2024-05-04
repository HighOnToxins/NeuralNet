namespace NeuralNet.Program;

public interface ITrainingProgram
{

    public void Run(INet net, int iterations, string savePath);

    //TODO: Add saving of data to files methods

    // save parameters/variables

}