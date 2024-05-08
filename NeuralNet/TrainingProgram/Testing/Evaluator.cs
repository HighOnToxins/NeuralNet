using NeuralNet.Tensor;

namespace NeuralNet.TrainingProgram.Testing;

/// <summary> Allows one to construct a confusion matrix over the categories optimized over. </summary>
public sealed class Evaluator
{
    private readonly Vector[][] testingData;

    private readonly Func<Vector, Vector> guessInterpreter;


    public Evaluator(Vector[][] testingData, Func<Vector, Vector> guessInterpreter, int categoryCount)
    {
        this.testingData = testingData;
        this.guessInterpreter = guessInterpreter;

        CategoryCount = categoryCount;
    }



    public int CategoryCount { get; private init; }

    private Vector SummedPrediction(INet net, int category)
    {
        Vector total = new(CategoryCount);
        Vector[] data = testingData[category];
        for (int j = 0; j < data.Length; j++)
        {
            total += guessInterpreter.Invoke(net.Run(data[j]));
        }

        return total;
    }

    public Matrix ConfusionMatrix(INet net)
    {
        Vector[] matrixRows = new Vector[CategoryCount];

        for (int i = 0; i < CategoryCount; i++)
        {
            matrixRows[i] = SummedPrediction(net, i);
        }

        return new(matrixRows, false);
    }



    public static float TruePositive(Matrix confusionMatrix, int category)
    {
        return confusionMatrix[category, category];
    }

    public static float TrueNegative(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            for (int j = 0; j < confusionMatrix.Width; j++)
            {
                if (i == category || j == category) continue;

                total += confusionMatrix[i, j];
            }
        }

        return total;
    }

    public static float FalsePositive(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            if (i == category) continue;
            total += confusionMatrix[i, category];
        }

        return total;
    }

    public static float FalseNegative(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Width; i++)
        {
            if (i == category) continue;
            total += confusionMatrix[category, i];
        }

        return total;
    }

    // T = TP + TN
    public static float True(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            for (int j = 0; j < confusionMatrix.Width; j++)
            {
                if (i == category ^ j == category) continue;

                total += confusionMatrix[i, j];
            }
        }

        return total;
    }

    // P = TP + FP (guess)
    public static float Positive(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            total += confusionMatrix[i, category];
        }

        return total;
    }

    // C = TP + FN
    public static float Category(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Width; i++)
        {
            total += confusionMatrix[category, i];
        }

        return total;
    }

    // Total - C
    public static float NotCategory(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            if (i == category) continue;
            for (int j = 0; j < confusionMatrix.Width; j++)
            {
                total += confusionMatrix[i, j];
            }
        }

        return total;
    }



    // TP + TN + FP + FN = N + P = T + F 
    public static float Total(Matrix confusionMatrix)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            for (int j = 0; j < confusionMatrix.Width; j++)
            {
                total += confusionMatrix[i, j];
            }
        }

        return total;
    }

    public static float TotalTrue(Matrix confusionMatrix)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Width; i++)
        {
            total += confusionMatrix[i, i];
        }

        return total;
    }

    public static float TotalFalse(Matrix confusionMatrix)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            for (int j = 0; j < confusionMatrix.Width; j++)
            {
                if (i == j) continue;
                total += confusionMatrix[i, j];
            }
        }

        return total;
    }



    /// <summary> Measure the percentage of correct classifications. </summary>
    public static float Accuracy(Matrix confusionMatrix, int category)
    {
        float fullTrue = True(confusionMatrix, category);
        float total = Total(confusionMatrix);

        return fullTrue / total;
    }

    /// <summary> Measure the estimated performance without True-Negative. </summary>
    public static float Precision(Matrix confusionMatrix, int category)
    {
        float truePositive = TruePositive(confusionMatrix, category);
        float positive = Positive(confusionMatrix, category);

        return truePositive / positive;
    }

    /// <summary> Measure the performance on that category isolated (sensitivity of the model). </summary>
    public static float Recall(Matrix confusionMatrix, int category)
    {
        float truePositive = TruePositive(confusionMatrix, category);
        float totalCategory = Category(confusionMatrix, category);

        return truePositive / totalCategory;
    }

    /// <summary> Measure the performance on every except that category isolated (sensitivity of the model). </summary>
    public static float Specificity(Matrix confusionMatrix, int category)
    {
        float trueNegative = TrueNegative(confusionMatrix, category);
        float totalCategory = NotCategory(confusionMatrix, category);

        return trueNegative / totalCategory;
    }



    /// <summary> The by-category average accuracy. </summary>
    public static float MacroAccuracy(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            total += Accuracy(confusionMatrix, category);
        }

        return total;
    }

    /// <summary> The by-category average precision. </summary>
    public static float MacroPrecision(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            total += Precision(confusionMatrix, category);
        }

        return total;
    }

    /// <summary> The by-category average recall. </summary>
    public static float MacroRecall(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            total += Recall(confusionMatrix, category);
        }

        return total;
    }

    /// <summary> The by-category average specificity. </summary>
    public static float MacroSpecificity(Matrix confusionMatrix, int category)
    {
        float total = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            total += Specificity(confusionMatrix, category);
        }

        return total;
    }



    /// <summary> The full accuracy/precision/recall over the entire matrix. </summary>
    public static float MicroAccuracy(Matrix confusionMatrix, int category)
    {
        float fullTrue = TotalTrue(confusionMatrix);
        float total = Total(confusionMatrix);

        return fullTrue / total;
    }

}
