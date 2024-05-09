
using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram.Display;

public sealed class EvaluationMeasure: IMeasure
{
    private readonly CategoryTester tester;
    
    private readonly bool measureAccuracy;
    private readonly bool measurePrecision;
    private readonly bool measureRecall;
    private readonly bool measureSpecificity; 

    public EvaluationMeasure(CategoryTester tester, bool measureAccuracy = false, bool measurePrecision = false, bool measureRecall = false, bool measureSpecificity = false)
    {
        this.tester = tester;

        this.measureAccuracy = measureAccuracy;
        this.measurePrecision = measurePrecision;
        this.measureRecall = measureRecall;
        this.measureSpecificity = measureSpecificity;
    }


    public IEnumerable<string> CollectFields()
    {
        if(measureAccuracy)
        {
            yield return "Micro Accuracy";
        }

        if(measurePrecision)
        {
            yield return "Macro Precision";
        }

        if(measureRecall)
        {
            yield return "Macro Recall";
        }

        if(measureSpecificity)
        {
            yield return "Macro Specificity";
        }
    }

    public IEnumerable<object> CollectInfo(INet net)
    {
        Matrix confusionMatrix = tester.ConfusionMatrix(net);

        if(measureAccuracy)
        {
            yield return CategoryTester.MicroAccuracy(confusionMatrix);
        }

        if(measurePrecision)
        {
            yield return CategoryTester.MacroPrecision(confusionMatrix);
        }

        if(measureRecall)
        {
            yield return CategoryTester.MacroRecall(confusionMatrix);
        }

        if(measureSpecificity)
        {
            yield return CategoryTester.MacroSpecificity(confusionMatrix);
        }
    }
}
