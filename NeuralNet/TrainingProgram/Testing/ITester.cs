namespace NeuralNet.TrainingProgram.Testing;

public interface ITester<T> : ITester
{

    public float Loss(T net);

    float ITester.Loss(INet net) => Loss(net);

}

public interface ITester
{

    public float Loss(INet net);

}
