using NeuralNet.Tensor;

namespace NeuralNet.Feedforward.Layers;

public sealed class ConvolutionalLayer: IFeedForwardLayer
{
    public int InputSize { get => imageWidth*imageHeight; }
    public int OutputSize { get => imageWidth*imageHeight; }

    private Vector weights;

    private readonly int imageWidth;
    private readonly int imageHeight;

    private readonly int kernelWidth;
    private readonly int kernelHeight;

    private readonly IActivation activation;

    public ConvolutionalLayer(int imageWidth, int imageHeight, int kernelWidth, int kernelHeight, IActivation? activation = null)
    {
        this.activation = activation ?? new Identity();

        this.imageWidth = imageWidth;
        this.imageHeight = imageHeight;
        
        this.kernelWidth = kernelWidth;
        this.kernelHeight = kernelHeight;

        weights = new(GetWeightLength());
    }

    private Matrix WeightGradient(Vector input)
    {
        float[,] output = new float[input.Height, GetWeightLength()];

        for(int outIndex = 0; outIndex < output.GetLength(0); outIndex++)
        {
            for(int kernelIndex = 0; kernelIndex < output.GetLength(1); kernelIndex++)
            {
                int inIndex = InIndex(outIndex, kernelIndex);
                if(inIndex == -1) continue;
                output[outIndex, kernelIndex] = input[inIndex];
            }
        }

        return new(output);
    }

    private Matrix InputGradient()
    {
        float[,] output = new float[InputSize, InputSize];

        for(int outIndex = 0; outIndex < output.GetLength(0); outIndex++)
        {
            for(int inIndex = 0; inIndex < output.GetLength(1); inIndex++)
            {
                int kernelIndex = KernelIndex(outIndex, inIndex);
                if(kernelIndex == -1) continue;
                output[outIndex, inIndex] = weights[kernelIndex];
            }
        }

        return new(output);
    }

    private Vector ComputeConvolution(Vector input)
    {
        float[] output = new float[OutputSize];

        for(int outIndex = 0; outIndex < InputSize; outIndex++)
        {
            float total = 0;

            for(int kernelIndex = 0; kernelIndex < GetWeightLength(); kernelIndex++)
            {
                int inIndex = InIndex(outIndex, kernelIndex);
                if(inIndex != -1)
                {
                    total += input[inIndex] * weights[kernelIndex];
                }

                // otherwise add zero
            }

            output[outIndex] = total;
        }

        return new(output);
    }

    public Matrix Gradient(Vector input, out Matrix inputGradient, out Vector result)
    {
        Vector convolution = ComputeConvolution(input);
        Vector activationGradient = activation.ComputeGradient(convolution);

        inputGradient = activationGradient.Scale(InputGradient());
        result = activation.ComputeGradient(convolution);
        return activationGradient.Scale(WeightGradient(input));
    }

    public Vector Run(Vector input)
    {
        return activation.ComputeGradient(ComputeConvolution(input));
    }

    private int InIndex(int outIndex, int kernelIndex)
    {
        int outImageX = outIndex % imageWidth;
        int outImageY = outIndex / imageWidth;

        int kernelX = kernelIndex % kernelWidth;
        int kernelY = kernelIndex / kernelWidth;

        int inImageX = outImageX - (int)Math.Floor(kernelWidth / 2f) + kernelX;
        int inImageY = outImageY - (int)Math.Floor(kernelHeight / 2f) + kernelY;

        if(0 <= inImageX && inImageX < imageWidth && 0 <= inImageY && inImageY < imageHeight)
        {
            int inIndex = inImageX + imageWidth * inImageY;
            return inIndex;
        }

        return -1;
    }
    private int KernelIndex(int outIndex, int inIndex)
    {
        int inImageX = inIndex % imageWidth;
        int inImageY = inIndex / imageWidth;

        int outImageX = outIndex % imageWidth;
        int outImageY = outIndex / imageWidth;

        int kernelX = inImageX - outImageX + (int)Math.Floor(kernelWidth / 2f);
        int kernelY = inImageY - outImageY + (int)Math.Floor(kernelHeight / 2f);

        if(0 <= kernelX && kernelX < kernelWidth && 0 <= kernelY && kernelY < kernelHeight)
        {
            int kernelIndex = kernelX + kernelWidth * kernelY;
            return kernelIndex;
        }

        return -1;
    }

    public void SetWeights(Vector newWeights)
    {
        weights = newWeights;
    }

    public void AddWeights(Vector newWeights)
    {
        weights += newWeights;
    }

    public int GetWeightLength()
    {
        return kernelWidth * kernelHeight;
    }

    public Vector GetWeights()
    {
        return weights;
    }

}
