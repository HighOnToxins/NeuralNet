
using NeuralNet;
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;

namespace Training;

internal class Program
{
    private const string mnistDirectory = "../../../MNISTfiles/";
    private const string netDirectory = "../../../net/";
    private const string netPath = netDirectory + "gradientDenseNet";

    private const int dataUse = -1;

    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(float[] output1, float[] output2)
        {
            float totalDifference = 0;
            for(int i = 0; i < output1.Length; i++)
            {
                totalDifference += Math.Abs(output2[i] - output1[i]);
            }
            return totalDifference;
        }

        public float[] Gradient(float[] output1, float[] output2)
        {
            float[] gradient = new float[output1.Length];
            for(int i = 0; i < output1.Length; i++)
            {
                gradient[i] = Math.Sign(output2[i] - output1[i]);
            }
            return gradient;
        }
    }

    private static float[][] AssignImageData(byte[][,] images)
    {
        float[][] values = new float[dataUse >= 0 ? dataUse : images.Length][];

        for(int i = 0; i <  values.Length; i++)
        {
            values[i] = new float[images[i].GetLength(0)*images[i].GetLength(1)];
            for(int j = 0; j < values[i].Length; j++)
            {
                values[i][j] = images[i][j % images[i].GetLength(0), j / images[i].GetLength(0)] / 255f;
            }
        }

        return values;
    }

    private static float[][] AssignLabelData(byte[] labels)
    {
        float[][] values = new float[dataUse >= 0 ? dataUse : labels.Length][];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = new float[10];
            values[i][ labels[i] ] = 1;
        }

        return values;
    }

    public static void Main(string[] _)
    {
        byte[]    trainLabels = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.trainingData);
        byte[][,] trainImages = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.trainingData);
        //byte[]    testLabels  = MNISTLoader.LoadLabels(mnistDirectory, MNISTLoader.LoadType.testingData );
        //byte[][,] testImages  = MNISTLoader.LoadImages(mnistDirectory, MNISTLoader.LoadType.testingData );

        //converting data into a usable form
        float[][] trainingTargetData = AssignLabelData(trainLabels);
        float[][] trainingInputData  = AssignImageData(trainImages);
        //float[][] testingTargetData  = AssignLabelData(testLabels);
        //float[][] testingInputData   = AssignImageData(testImages);

        // TODO: add training run for MNIST network

        Console.WriteLine("Loaded files!");

        GradientDecentTrainer trainer = new(trainingInputData, trainingTargetData, new LossFunction());

        //TODO:Consider downscaling images
        FeedForwardNet net = new(
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU(.05f))
        );

        if(!File.Exists(netPath + ".bin"))
        {
            Directory.CreateDirectory(netDirectory);
            net.Randomize(x => 500f * (float) Math.Pow(x - .5f, 7));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        RunMomentumTraining(trainer, net, 100, 1f / dataUse, .25f);
    }

    //using momentum
    public static void RunMomentumTraining(GradientDecentTrainer trainer, FeedForwardNet net, int iterations, float learningRate, float decay)
    {
        float[] velocity = new float[net.GetWeightLength()];

        Console.WriteLine();
        Console.WriteLine($"number of iterations: {iterations} ");
        Console.WriteLine($"learning rate: {learningRate}");
        Console.WriteLine($"decay: {decay}");
        Console.WriteLine();

        Console.WriteLine("Started training!");
        Console.WriteLine(string.Format("\n{0,20}\t{1,20}\t{2,20}\t{3,20}", "Iteration", "Loss", "Speed", "Acceleration"));

        FeedForwardNet best = net.Clone();
        float bestLoss = trainer.Loss(best);

        for(int i = 0; i < iterations; i++)
        {
            //train
            float[] acceleration = trainer.Train(net);
            for(int j = 0; j < velocity.Length; j++) 
                velocity[j] = velocity[j] * decay + acceleration[j]*learningRate;
            net.AddWeights(velocity);

            //save best
            float loss = trainer.Loss(net);
            if(loss > bestLoss)
            {
                bestLoss = loss;
                best = net.Clone();
                best.Save(netPath);
            }

            //log
            Console.WriteLine(string.Format("{0,20}\t{1,20}\t{2,20}\t{3,20}",
                i, loss, Matrix.Length(velocity), Matrix.Length(acceleration)));

        }

        net.Save(netPath);
        Console.WriteLine("saved net!");
        Console.WriteLine("training ended!");
    }

}
