using NeuralNet.Tensor;

namespace NeuralNet.Feedforward;

public sealed class FeedForwardTrainer: ITrainer<FeedforwardNet>
{

    public const int THREADCOUNT = 10;

    private readonly Vector[] inputs;
    private readonly Vector[] targets;

    private readonly IFeedForwardLoss loss;

    public FeedForwardTrainer(Vector[] inputs, Vector[] targets, IFeedForwardLoss loss)
    {
        if(inputs.Length != targets.Length)
        {
            throw new ArgumentException();
        }

        this.inputs = inputs;
        this.targets = targets;
        this.loss = loss;
    }

    public Vector Train(FeedforwardNet net)
    {
        if(THREADCOUNT > 1 && inputs.Length >= THREADCOUNT)
        {
            Thread[] thread = new Thread[THREADCOUNT];
            Vector[] results = new Vector[THREADCOUNT];

            for(int i = 0; i < THREADCOUNT; i++)
            {
                thread[i] = new(num => ThreadedGradient(net, loss, inputs, targets, results, num));
                thread[i].Start(i);
            }

            for(int i = 0; i < THREADCOUNT; i++)
            {
                thread[i].Join();
            }

            Vector threadedTotalGradient = results[0];
            
            for(int i = 1; i < results.Length; i++)
            {
                threadedTotalGradient += results[i];
            }

            return threadedTotalGradient;
        }

        Vector totalGradient = new(net.GetWeightLength());

        for(int i = 0; i < inputs.Length; i++)
        {
            Matrix gradient = net.Gradient(inputs[i], out Vector run);
            totalGradient += gradient.Transpose() * loss.Gradient(targets[i], run);
        }

        return totalGradient;
    }

    private static void ThreadedGradient(
        FeedforwardNet net, 
        IFeedForwardLoss loss, 
        Vector[] inputs, 
        Vector[] targets, 
        Vector[] results, 
        object? threadNum)
    {
        if(threadNum is not int num)
        {
            throw new ArgumentException();
        }

        results[num] = new(net.GetWeightLength());

        float unit = inputs.Length / THREADCOUNT;
        int start = (int) (unit * num);
        int end = (int)(unit * (num+1));

        for(int i = start; i < end; i++)
        {
            Matrix gradient = net.Gradient(inputs[i], out Vector run);
            results[num] += gradient.Transpose() * loss.Gradient(targets[i], run);
        }
    }

    public float Loss(INet net)
    {
        float total = 0;

        for(int i = 0; i < inputs.Length; i++)
        {
            total += loss.Compute(targets[i], net.Run(inputs[i]));
        }

        return total;
    }

}
