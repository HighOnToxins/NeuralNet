
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Program;

internal class Start
{
    const string mnistDirectory = "../../../MNISTfiles/";
    const string netPath = "../../../net/gradientDenseNet.bin";

    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(float[] output1, float[] output2)
        {
            throw new NotImplementedException();
        }

        public float[] Gradient(float[] output1, float[] output2)
        {
            throw new NotImplementedException();
        }
    }

    private static float[][] AssignImageData(byte[][,] images)
    {
        float[][] values = new float[images.Length][];

        for(int i = 0; i <  images.Length; i++)
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
        float[][] values = new float[labels.Length][];

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

        FeedForwardNet net = new(
            new AffineLayer(28*28, 100, new ReLU()),
            new AffineLayer(100, 100, new ReLU()),
            new AffineLayer(100, 100, new ReLU()),
            new AffineLayer(100, 100, new ReLU()),
            new AffineLayer(100, 10, new ReLU())
        );

        if(!File.Exists(netPath))
        {
            Directory.CreateDirectory(netPath);
            net.Randomize(x => 800f * (float) Math.Pow(x - .5f, 3));
            net.Save(netPath);
        }
        else
        {
            net.Load(netPath);
        }

        Console.WriteLine("Started training!");

        //using momentum
        float speed = 10;
        for(int i = 0; i < 100; i++)
        {
            speed += trainer.Train(net, speed);
            Console.WriteLine(trainer.Loss(net));
        }

    }
}
