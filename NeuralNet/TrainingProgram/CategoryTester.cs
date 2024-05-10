using NeuralNet.Tensor;
using System.Security.Principal;

namespace NeuralNet.TrainingProgram;

/// <summary> Allows one to construct a confusion matrix over the categories optimized over. </summary>
public sealed class CategoryTester : ITester
{
    private readonly Vector[][] testingData;

    private readonly Vector[] labels;

    private readonly ILoss loss;

    private readonly Func<Vector, Vector> guessInterpreter;

    public CategoryTester(Vector[][] testingData, Vector[] labels, ILoss loss, Func<Vector, Vector> guessInterpreter, int categoryCount)
    {
        this.testingData = testingData;
        this.labels = labels;
        this.loss = loss;

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

    public float Loss(INet net)
    {
        float totalLoss = 0;

        for (int i = 0; i < testingData.Length; i++)
        {
            for (int j = 0; j < testingData[i].Length; j++)
            {
                totalLoss += loss.Compute(labels[i], net.Run(testingData[i][j]));
            }
        }

        return totalLoss;
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
    public static float MacroAccuracy(Matrix confusionMatrix)
    {
        float total = 0;
        int nanCount = 0;

        for(int i = 0; i < confusionMatrix.Height; i++)
        {
            float value = Accuracy(confusionMatrix, i);
            if(float.IsNaN(value))
            {
                nanCount++;
                continue;
            }

            total += value;
        }

        return total / (confusionMatrix.Height - nanCount);
    }

    /// <summary> The by-category average precision. </summary>
    public static float MacroPrecision(Matrix confusionMatrix)
    {
        float total = 0;
        int nanCount = 0;

        for (int i = 0; i < confusionMatrix.Height; i++)
        {
            float value = Precision(confusionMatrix, i);
            if(float.IsNaN(value))
            {
                nanCount++;
                continue;
            }

            total += value;
        }

        return total / (confusionMatrix.Height - nanCount);
    }

    /// <summary> The by-category average recall. </summary>
    public static float MacroRecall(Matrix confusionMatrix)
    {
        float total = 0;
        int nanCount = 0;

        for(int i = 0; i < confusionMatrix.Height; i++)
        {
            float value = Recall(confusionMatrix, i);
            if(float.IsNaN(value))
            {
                nanCount++;
                continue;
            }

            total += value;
        }

        return total / (confusionMatrix.Height - nanCount);
    }

    /// <summary> The by-category average specificity. </summary>
    public static float MacroSpecificity(Matrix confusionMatrix)
    {
        float total = 0;
        int nanCount = 0;

        for(int i = 0; i < confusionMatrix.Height; i++)
        {
            float value = Specificity(confusionMatrix, i);
            if(float.IsNaN(value))
            {
                nanCount++;
                continue;
            }

            total += value;
        }

        return total / (confusionMatrix.Height - nanCount);
    }



    /// <summary> The full accuracy/precision/recall over the entire matrix. </summary>
    public static float MicroAccuracy(Matrix confusionMatrix)
    {
        float fullTrue = TotalTrue(confusionMatrix);
        float total = Total(confusionMatrix);

        return fullTrue / total;
    }

}
