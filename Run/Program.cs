
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using Training;

FeedForwardNet net = new(
    new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU())
);


