
namespace NeuralNet.Program;

public sealed class NewtonProgram: ITrainingProgram
{

    private readonly ITrainer trainer;

    public NewtonProgram(ITrainer trainer)
    {
        this.trainer = trainer;
    }

    public void Run(INet net, int iterations, string savePath)
    {
        Console.WriteLine("Started training!");
        Console.WriteLine(string.Format("\n{0,20}\t{1,20}\t{2,20}", "Iteration", "Loss", "Delta"));

        INet best = net.Clone();
        float bestLoss = trainer.Loss(best);

        for(int i = 0; i < iterations; i++)
        {
            //train
            (float[] gradient, float loss) = trainer.Train(net);
            float[] delta = gradient.Scale(loss / gradient.LengthSquared());
            net.AddWeights( delta );

            //save best
            if(loss > bestLoss)
            {
                bestLoss = loss;
                best = net.Clone();
                best.Save(savePath);
            }

            //log
            Console.WriteLine(string.Format("{0,20}\t{1,20}\t{2,20}", i, loss, delta));
        }

        net.Save(savePath);

        Console.WriteLine("saved net!");
        Console.WriteLine("training ended!");
    }
}
