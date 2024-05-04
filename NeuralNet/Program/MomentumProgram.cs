
namespace NeuralNet.Program;

public sealed class MomentumProgram : ITrainingProgram
{
    private readonly ITrainer trainer;

    private readonly float learningRate;
    private readonly float decay;

    public MomentumProgram(ITrainer trainer, float learningRate, float decay)
    {
        this.trainer = trainer;

        this.learningRate = learningRate;
        this.decay = decay;
    }

    public void Run(INet net, int iterations, string savePath)
    {
        float[] velocity = new float[net.GetWeightLength()];

        Console.WriteLine();
        Console.WriteLine($"number of iterations: {iterations} ");
        Console.WriteLine($"learning rate: {learningRate}");
        Console.WriteLine($"decay: {decay}");
        Console.WriteLine();

        Console.WriteLine("Started training!");
        Console.WriteLine(string.Format("\n{0,20}\t{1,20}\t{2,20}\t{3,20}", "Iteration", "Loss", "Speed", "Acceleration"));

        INet best = net.Clone();
        float bestLoss = trainer.Loss(best);

        for(int i = 0; i < iterations; i++)
        {
            //train
            (float[] acceleration, float loss) = trainer.Train(net);
            for(int j = 0; j < velocity.Length; j++)
                velocity[j] = velocity[j] * decay + acceleration[j] * learningRate;
            net.AddWeights(velocity);

            //save best
            if(loss > bestLoss)
            {
                bestLoss = loss;
                best = net.Clone();
                best.Save(savePath);
            }

            //log
            Console.WriteLine(string.Format("{0,20}\t{1,20}\t{2,20}\t{3,20}",
                i, loss, velocity.Length(), acceleration.Length()));

        }

        net.Save(savePath);

        Console.WriteLine("saved net!");
        Console.WriteLine("training ended!");
    }

    //TODO: Consider making saving and logging seperate from the program
    // add a logger
    // add saver
}
