
using NeuralNet.Feedforward;
using NeuralNet.Feedforward.Layers;
using Training;

FeedforwardNet net = new(
    new AffineLayer(MNISTLoader.ImageSize*MNISTLoader.ImageSize, 10, new ReLU())
);

//TODO: Check that the network works?
//TODO: Add verification and other test stuff
