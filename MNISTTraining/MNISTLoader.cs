﻿
namespace MNSITTraining;

public static class MNISTLoader
{
    public enum LoadType { testingData, trainingData }
    public const string webPath = "http://yann.lecun.com/exdb/mnist/";

    public const string labelType = "idx1-ubyte";
    public const string imageType = "idx3-ubyte";

    public const string trainLabelName = "train-labels";
    public const string trainImageName = "train-images";
    public const string testLabelName = "t10k-labels";
    public const string testImageName = "t10k-images";

    public const int ImageSize = 28;

    public const int CategoryCount = 10;

    public static byte[] LoadLabels(string directory, LoadType type)
    {
        using BinaryReader reader = new(File.OpenRead(directory + (type == LoadType.testingData ? testLabelName : trainLabelName) + "." + labelType));
        
        int magicNumber = reader.ReadInt32().FromBigEndian();
        if(magicNumber != 2049) throw new ArgumentException("Could not read file, magic number was wrong!");

        int itemCount = reader.ReadInt32().FromBigEndian();
        byte[] labels = new byte[itemCount];
        for(int i = 0; i < itemCount; i++)
        {
            labels[i] = reader.ReadByte();
        }

        return labels;
    }

    public static byte[][,] LoadImages(string directory, LoadType type)
    {
        using BinaryReader reader = new(File.OpenRead(directory + (type == LoadType.testingData ? testImageName : trainImageName) + "." + imageType));
        
        int magicNumber = reader.ReadInt32().FromBigEndian();
        if(magicNumber != 2051) throw new ArgumentException("Could not read file, magic number was wrong!");

        int itemCount = reader.ReadInt32().FromBigEndian();
        int rowCount = reader.ReadInt32().FromBigEndian();
        int colCount = reader.ReadInt32().FromBigEndian();

        byte[][,] images = new byte[itemCount][,];
        for(int i = 0; i < itemCount; i++)
        {
            images[i] = new byte[colCount, rowCount];

            for(int x = 0; x < images[i].GetLength(0); x++)
            {
                for(int y = 0; y < images[i].GetLength(0); y++)
                {
                    images[i][x, y] = reader.ReadByte();
                }
            }
        }

        return images;
    }

    public static void CountLabels(string directory, string outputPath)
    {
        //load data
        byte[] data = LoadLabels(directory, LoadType.trainingData);

        //count
        int[] counts = new int[10];
        for(int i = 0; i < data.Length; i++)
        {
            counts[data[i]]++;
        }

        //write
        StreamWriter writer = new(File.Create(outputPath));

        for(int i = 0; i < counts.Length; i++)
        {
            writer.Write(i + "; ");
        }

        writer.WriteLine();

        for(int i = 0; i < counts.Length; i++)
        {
            writer.Write(counts[i] + "; ");
        }

        writer.Close();
    }

    async public static void DownloadMNIST(string directory)
    {
        using var client = new HttpClient();
        Task<byte[]> trainLabelsBytes = client.GetByteArrayAsync(webPath + trainLabelName + "-" + labelType + ".gz");
        Task<byte[]> trainDataBytes = client.GetByteArrayAsync(webPath + trainImageName + "-" + imageType + ".gz");
        Task<byte[]> testLabelsBytes = client.GetByteArrayAsync(webPath + testLabelName + "-" + labelType + ".gz");
        Task<byte[]> testDataBytes = client.GetByteArrayAsync(webPath + testImageName + "-" + imageType + ".gz");

        Directory.CreateDirectory(directory);
        using(BinaryWriter writer = new(File.Create(directory + trainLabelName + "." + labelType))) writer.Write(await trainLabelsBytes);
        using(BinaryWriter writer = new(File.Create(directory + trainImageName + "." + imageType))) writer.Write(await trainDataBytes);
        using(BinaryWriter writer = new(File.Create(directory + testLabelName + "." + labelType))) writer.Write(await testLabelsBytes);
        using(BinaryWriter writer = new(File.Create(directory + testImageName + "." + imageType))) writer.Write(await testDataBytes);
    }

}