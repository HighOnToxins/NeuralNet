This code was made in conjunction with a 6th semester bachelor project of Aalborg University.

Here you will find four diffent project solutions, and one ProjectRuns folder.

Running Code
The code was made in visual studio and it might be easiest to run it from there using the NeuralNet.sln file.
Otherwise in each project folder there is a bin folder, 
within that chose debug or release, net6.0 and then from the comand-line call the <project name>.exe file.

NeuralNet
This is the main library, it includes the basic need for running a neural network and for saving data while doing so.

NetTest
Here are a few test regarding the NeuralNet project. Classes such as the feedforward net itself and the matrix, along with many other are tested here.

Benchmarking
This project is not well organized but it includes the benchmarks that where done for this project, mainly on the matrix struct.

MNISTTraining
Here the MNIST data is loaded and networks are created and run. Anything run from here will be saved in the ProjectRuns folder.
