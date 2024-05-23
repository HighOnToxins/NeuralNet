using NeuralNet.Tensor;
using System.Linq.Expressions;

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
            Exception[] exceptions = new Exception[THREADCOUNT];

            results[0] = new(net.GetWeightLength());
            for(int i = 1; i < THREADCOUNT; i++)  results[i] = results[0];

            void ThreadRun(object? num) => ThreadedGradient(net, loss, inputs, targets, results, (int)num, exceptions);
            
            // setting up threads
            for(int i = 0; i < THREADCOUNT; i++)
            {
                thread[i] = new(ThreadRun);
                thread[i].Start(i);
            }

            //waiting for threads
            for(int i = 0; i < THREADCOUNT; i++)
            {
                thread[i].Join();

                //handling exceptions
                if(exceptions[i] is not null)
                {
                    throw exceptions[i];
                }
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
        int threadNum,
        Exception[] exceptions)
    {
        try
        {
            float unit = inputs.Length / THREADCOUNT;
            int start = (int)(unit * threadNum);
            int end = (int)(unit * (threadNum + 1));

            for(int i = start; i < end; i++)
            {
                Matrix gradient = net.Gradient(inputs[i], out Vector run);
                results[threadNum] += gradient.Transpose() * loss.Gradient(targets[i], run);
            }
        }
        catch(Exception e) 
        {
            //exception handling
            exceptions[threadNum] = e;
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
