﻿namespace NeuralNet;

public interface IActivation
{

    public float[] Run(float[] input);

    public float[,] ComputeGradient(float[] input);

}