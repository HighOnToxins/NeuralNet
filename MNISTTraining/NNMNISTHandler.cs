
using MNSITTraining;
using NeuralNet;
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using NeuralNet.Tensor;
using NeuralNet.TrainingProgram;
using System;

namespace MNISTTraining;

public class NNMNISTHandler
{

    private const int dataUse = -1;

    public NNMNISTHandler(string directory, bool useMaxGuess = true) 
    {

        //converting data into a usable form
        Vector[] trainingInputData = TrainingImages(directory);
        Vector[] trainingTargets = TrainingLabels(directory);

        Vector[][] testingInputData = TestingData(directory);
        Vector[] testingLabels = TestingLabels();

        Console.WriteLine("Loaded files!");

        //trainer, tester & program
        LossFunction loss = new();

        Trainer = new(trainingInputData, trainingTargets, loss);
        Tester = new(testingInputData, testingLabels, loss, useMaxGuess ? MaxGuess : FullGuess, MNISTLoader.CategoryCount);

    }

    public FeedForwardTrainer Trainer { get; private init; }
    public CategoryTester Tester { get; private init; }

    public static Vector MaxGuess(Vector output)
    {
        int maxIndex = 0;
        for(int i = 1; i < output.Height; i++)
        {
            if(output[i] > maxIndex)
            {
                maxIndex = i;
            }
        }

        float[] result = new float[MNISTLoader.CategoryCount];
        result[maxIndex] = 1;
        return new(result);
    }

    public static Vector FullGuess(Vector output)
    {
        return output;
    }


    public class LossFunction: IFeedForwardLoss
    {
        public float Compute(Vector output1, Vector output2)
        {
            float totalDifference = 0;
            for(int i = 0; i < output1.Height; i++)
            {
                totalDifference += Math.Abs(output2[i] - output1[i]);
            }
            return totalDifference;
        }

        public Vector Gradient(Vector output1, Vector output2)
        {
            float[] gradient = new float[output1.Height];
            for(int i = 0; i < output1.Height; i++)
            {
                gradient[i] = Math.Sign(output2[i] - output1[i]);
            }
            return new Vector(gradient);
        }
    }

    public static Vector[] TrainingImages(string directory)
    {
        byte[][,] images = MNISTLoader.LoadImages(directory, MNISTLoader.LoadType.trainingData);

        int maxUse = dataUse >= 0 ? dataUse : images.Length;

        Vector[] data = new Vector[maxUse];

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[MNISTLoader.ImageSize * MNISTLoader.ImageSize];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = images[i][j % MNISTLoader.ImageSize, j / MNISTLoader.ImageSize] / 255f;
            }
            data[i] = new(vecValues);
        }

        return data;
    }

    public static Vector[] TrainingLabels(string directory)
    {
        byte[] labels = MNISTLoader.LoadLabels(directory, MNISTLoader.LoadType.trainingData);

        int maxUse = dataUse >= 0 ? dataUse : labels.Length;

        Vector[] data = new Vector[maxUse];

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[MNISTLoader.CategoryCount];
            vecValues[labels[i]] = 1;
            data[i] = new(vecValues);
        }

        return data;
    }

    public static Vector[][] TestingData(string directory)
    {
        byte[][,] images = MNISTLoader.LoadImages(directory, MNISTLoader.LoadType.testingData);
        byte[] labels = MNISTLoader.LoadLabels(directory, MNISTLoader.LoadType.trainingData);

        int maxUse = dataUse >= 0 ? dataUse : images.Length;

        List<Vector>[] inputList = new List<Vector>[MNISTLoader.CategoryCount];
        for(int i = 0; i < MNISTLoader.CategoryCount; i++)
        {
            inputList[i] = new();
        }

        for(int i = 0; i < maxUse; i++)
        {
            float[] vecValues = new float[MNISTLoader.ImageSize * MNISTLoader.ImageSize];
            for(int j = 0; j < vecValues.Length; j++)
            {
                vecValues[j] = images[i][j % MNISTLoader.ImageSize, j / MNISTLoader.ImageSize] / 255f;
            }
            inputList[labels[i]].Add(new Vector(vecValues));
        }

        Vector[][] inputs = new Vector[MNISTLoader.CategoryCount][];

        for(int i = 0; i < MNISTLoader.CategoryCount; i++)
        {
            inputs[i] = inputList[i].ToArray();
        }

        return inputs;
    }

    public static Vector[] TestingLabels()
    {
        Vector[] result = new Vector[MNISTLoader.CategoryCount];

        for(int i = 0; i < MNISTLoader.CategoryCount; i++)
        {
            float[] floats = new float[MNISTLoader.CategoryCount];
            floats[i] = 1;
            result[i] = new(floats);
        }

        return result;
    }


    public static INet CreateMNISTNetwork()
    {
        IActivation activation = new ReLU(.05f);

        FeedforwardNet net = new(new IFeedForwardLayer[] {
            new ConvolutionalLayer(MNISTLoader.ImageSize, MNISTLoader.ImageSize, 3, 3, activation),
            new ConvolutionalLayer(MNISTLoader.ImageSize, MNISTLoader.ImageSize, 3, 3, activation),
            new ConvolutionalLayer(MNISTLoader.ImageSize, MNISTLoader.ImageSize, 3, 3, activation),
            new ConvolutionalLayer(MNISTLoader.ImageSize, MNISTLoader.ImageSize, 3, 3, activation),
            new ConvolutionalLayer(MNISTLoader.ImageSize, MNISTLoader.ImageSize, 3, 3, activation),
            new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, activation),
        });
        net.Randomize(x => 20f * (float)Math.Pow(x - .5f, 7));

        return net;
    }

}
