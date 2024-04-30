
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Program;

internal class Start
{
    private const string mnistDirectory = "../../../MNISTfiles/";
    private const string netDirectory = "../../../net/";
    private const string netPath = netDirectory + "gradientDenseNet";

    private const int dataUse = 1000;

    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(float[] output1, float[] output2)
        {
            float totalDifference = 0;
            for(int i = 0; i < output1.Length; i++)
            {
                totalDifference = Math.Abs(output1[i] - output2[i]);
            }
            return totalDifference;
        }

        public float[] Gradient(float[] output1, float[] output2)
        {
            float[] gradient = new float[10];
            for(int i = 0; i < output1.Length; i++)
            {
                gradient[i] = Math.Sign(output1[i] - output2[i]);
            }
            return gradient;
        }
    }

    private static float[][] AssignImageData(byte[][,] images)
    {
        float[][] values = new float[dataUse][];

        for(int i = 0; i <  values.Length; i++)
        {
            values[i] = new float[images[i].GetLength(0)*images[i].GetLength(1)];
            for(int j = 0; j < values[i].Length; j++)
            {
                values[i][j] = images[i][j % images[i].GetLength(0), j / images[i].GetLength(0)];
            }
        }

        return values;
    }

    private static float[][] AssignLabelData(byte[] labels)
    {
        float[][] values = new float[dataUse][];

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
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU())
        );

        if(!File.Exists(netPath + ".bin"))
        {
            Directory.CreateDirectory(netDirectory);
            net.Randomize(x => 800f * (float) Math.Pow(x - .5f, 3));
            Console.WriteLine("Created New Network!");
        }
        else
        {
            net.Load(netPath);
            Console.WriteLine("Loaded Network!");
        }

        Console.WriteLine("Started training!");

        //using momentum
        float speed = 10;
        float decay = .95f;

        Stopwatch timer = new();
        timer.Start();
        Console.WriteLine(String.Format("\n{0,18}   {1,18}   {2,18}   {3,18}", "Training Loss", "Current Speed", "Acceleration", "Saved Net"));
        for(int i = 0; i < 1000; i++)
        {
            float acceleration = trainer.Train(net, speed) / 100000f;
            speed += acceleration;
            speed *= decay;

            bool saved = false;
            if(timer.ElapsedMilliseconds >= 10000) //10 seconds before saving
            {
                net.Save(netPath);
                timer.Restart();
                saved = true;
            }

            Console.Write(String.Format("{0,18}   {1,18}   {2,18}   {3,18}\n", trainer.Loss(net), speed, acceleration, (saved ? "saved!" : "")));
        }

        net.Save(netPath);
        Console.WriteLine("saved net!");
        timer.Stop();
        Console.WriteLine("training ended!");
    }
}
