
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace Benchmarking.Tests;

public class VectorBenchmarks
{
    public readonly struct Matrix
    {
        internal readonly float[] values;



        public Matrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Height * Width];
        }



        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            internal set
            {
                values[row * Width + col] = value;
            }
        }

        public Matrix Transpose()
        {
            Matrix result = new(Width, Height);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    result[i, j] = this[j, i];
                }
            }

            return result;
        }

        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            Matrix C = new(A.Height, A.Width);
            for (int i = 0; i < A.Height; i++)
            {
                for (int j = 0; j < A.Width; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }

            return C;
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            Matrix B2 = B.Transpose();
            Matrix C = new(A.Height, B2.Height);

            for (int i = 0; i < A.Height; i++)
            {
                for (int j = 0; j < B2.Height; j++)
                {
                    float total = 0;

                    for (int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j, k];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

        public static Matrix operator *(Matrix A, float b)
        {
            Matrix C = new(A.Height, A.Width);
            for (int i = 0; i < A.Height; i++)
            {
                for (int j = 0; j < A.Width; j++)
                {
                    C[i, j] = A[i, j] * b;
                }
            }

            return C;
        }


        public string ToString(string rowSep = "\n", string colSep = "\t")
        {
            string total = "";
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    total += this[i, j].ToString() + colSep;
                }
                total += rowSep;
            }
            return total;
        }
    }



    public static Vector<float>[] GetValues(Matrix A)
    {
        int internalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        Vector<float>[] values = new Vector<float>[A.Height * internalWidth];

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < internalWidth; j++)
            {
                float[] floats = new float[Vector<float>.Count];
                int nonVectorIndex = j * Vector<float>.Count;
                int length = Math.Min(Vector<float>.Count, A.Width - nonVectorIndex);
                Buffer.BlockCopy(A.values, (i * A.Width + nonVectorIndex) * sizeof(float), floats, 0, length * sizeof(float));
                values[i * internalWidth + j] = new(floats);
            }
        }

        return values;
    }

    public static Vector<float>[] GetValuesTransposed(Matrix A)
    {
        int internalWidth = (int)Math.Ceiling(A.Height / (float)Vector<float>.Count);
        Vector<float>[] values = new Vector<float>[A.Width * internalWidth];

        for (int i = 0; i < A.Width; i++)
        {
            for (int j = 0; j < internalWidth; j++)
            {
                float[] floats = new float[Vector<float>.Count];
                int nonVectorIndex = j * Vector<float>.Count;
                int length = Math.Min(Vector<float>.Count, A.Height - nonVectorIndex);

                for (int k = 0; k < length; k++)
                {
                    floats[k] = A[nonVectorIndex + k, i];
                }

                values[i * internalWidth + j] = new(floats);
            }
        }

        return values;
    }

    public static Matrix GetMatrix(int height, int width, Vector<float>[] values)
    {
        int internalWidth = (int)Math.Ceiling(width / (float)Vector<float>.Count);
        Matrix result = new(height, width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < internalWidth; j++)
            {
                int length = width - j * Vector<float>.Count;
                if (length > Vector<float>.Count)
                {
                    values[i * internalWidth + j].CopyTo(result.values, i * width + j * Vector<float>.Count);
                    continue;
                }

                for (int k = 0; k < length; k++)
                {
                    result[i, j * Vector<float>.Count + k] = values[i * internalWidth + j][k];
                }
            }
        }

        return result;
    }



    public static Matrix OperatorAddPre(Matrix A, Matrix B)
    {
        if (A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        Vector<float>[] AValues = GetValues(A);
        Vector<float>[] BValues = GetValues(B);

        Vector<float>[] CValues = new Vector<float>[A.Height * AInternalWidth];

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < AInternalWidth; j++)
            {
                CValues[i * AInternalWidth + j] = AValues[i * AInternalWidth + j] + BValues[i * AInternalWidth + j];
            }
        }

        return GetMatrix(A.Width, A.Height, CValues);
    }

    public static Matrix OperatorProdPre(Matrix A, Matrix B)
    {
        if (A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        int BInternalWidth = (int)Math.Ceiling(B.Width / (float)Vector<float>.Count);
        int BTransposedInternalWidth = (int)Math.Ceiling(B.Height / (float)Vector<float>.Count);

        Vector<float>[] BValuesTransposed = GetValuesTransposed(B);
        Vector<float>[] AValues = GetValues(A);

        Vector<float>[] CValues = new Vector<float>[A.Height * BInternalWidth];

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < BInternalWidth; j++)
            {
                // vector dot product 
                float[] value = new float[Vector<float>.Count];

                int length = Math.Min(Vector<float>.Count, B.Width - j * Vector<float>.Count);
                for (int k = 0; k < length; k++)
                {
                    // single dot product 
                    Vector<float> totalVector = new();

                    for (int l = 0; l < AInternalWidth; l++)
                    {
                        totalVector += AValues[i * AInternalWidth + l] * BValuesTransposed[(j * Vector<float>.Count + k) * BTransposedInternalWidth + l];
                    }

                    float totalFloat = 0;
                    for (int NNN = 0; NNN < Vector<float>.Count; NNN++)
                    {
                        totalFloat += totalVector[NNN];
                    }

                    value[k] = totalFloat;
                }

                CValues[i * BTransposedInternalWidth + j] = new(value);
            }
        }

        return GetMatrix(A.Height, B.Width, CValues);
    }

    public static Matrix OperatorScalePre(Matrix A, float b)
    {
        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);
        Vector<float>[] AValues = GetValues(A);
        Vector<float>[] BValues = GetValues(A);

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < AInternalWidth; j++)
            {
                BValues[i * AInternalWidth + j] = AValues[i * AInternalWidth + j] * b;
            }
        }

        return GetMatrix(A.Width, A.Height, BValues);
    }


    public static Matrix OperatorAddIn(Matrix A, Matrix B)
    {
        if (A.Width != B.Width || A.Height != B.Height)
        {
            throw new ArgumentException("Expected matrices with equal lengthed sides!");
        }

        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        Matrix C = new(A.Height, A.Width);
        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < AInternalWidth; j++)
            {
                int length = Math.Min(Vector<float>.Count, A.Width - j * Vector<float>.Count);
                int offset = (i * A.Width + j * Vector<float>.Count) * sizeof(float);

                FastAdd<float>(
                    A.values, offset,
                    B.values, offset,
                    C.values, offset,
                    length * sizeof(float));
            }
        }

        return C;
    }

    public static Matrix OperatorProdIn(Matrix A, Matrix B)
    {
        if (A.Width != B.Height)
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        Matrix B2 = B.Transpose();
        Matrix C = new(A.Height, B2.Height);

        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < B.Width; j++)
            {
                C[i, j] = FastDotProduct(
                    A.values, i * A.Width * sizeof(float),
                    B2.values, j * B2.Width * sizeof(float),
                    A.Width * sizeof(float));
            }
        }

        return C;
    }

    public static Matrix OperatorScaleIn(Matrix A, float b)
    {
        int AInternalWidth = (int)Math.Ceiling(A.Width / (float)Vector<float>.Count);

        Matrix C = new(A.Height, A.Width);
        for (int i = 0; i < A.Height; i++)
        {
            for (int j = 0; j < AInternalWidth; j++)
            {
                int length = Math.Min(Vector<float>.Count, A.Width - j * Vector<float>.Count);
                int inputOffset = i * A.Width + j * Vector<float>.Count;

                FastScale(
                    A.values, inputOffset * sizeof(float),
                    b,
                    C.values, (i * A.Width + j * Vector<float>.Count) * sizeof(float),
                    length * sizeof(float));
            }
        }

        return C;
    }



    public static void FastAdd<T>(Array src1, int srcOffset1, Array src2, int srcOffset2, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];
        T[] BValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);
        Buffer.BlockCopy(src2, srcOffset2, BValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorB = new(BValues);

        Vector<T> vectorResult = vectorA + vectorB;

        T[] CValues = new T[Vector<float>.Count];
        vectorResult.CopyTo(CValues);
        Buffer.BlockCopy(CValues, 0, dst, dstOffset, count);
    }

    public static void FastMultiply<T>(Array src1, int srcOffset1, Array src2, int srcOffset2, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];
        T[] BValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);
        Buffer.BlockCopy(src2, srcOffset2, BValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorB = new(BValues);

        Vector<T> vectorResult = vectorA * vectorB;

        T[] resultValues = new T[Vector<T>.Count];
        vectorResult.CopyTo(resultValues);
        Buffer.BlockCopy(resultValues, 0, dst, dstOffset, count);
    }

    public static void FastScale<T>(Array src1, int srcOffset1, T scalar, Array dst, int dstOffset, int count) where T : struct
    {
        T[] AValues = new T[Vector<T>.Count];

        Buffer.BlockCopy(src1, srcOffset1, AValues, 0, count);

        Vector<T> vectorA = new(AValues);
        Vector<T> vectorResult = vectorA * scalar;

        T[] resultValues = new T[Vector<T>.Count];
        vectorResult.CopyTo(resultValues);
        Buffer.BlockCopy(resultValues, 0, dst, dstOffset, count);
    }

    private static float FastDotProduct(float[] src1, int srcOffset1, float[] src2, int srcOffset2, int count)
    {
        int valueCount = count / sizeof(float);
        int internalLength = (int)Math.Ceiling(valueCount / (float)Vector<float>.Count);

        Vector<float> totalVector = new();

        for (int l = 0; l < internalLength; l++)
        {
            int length = Math.Min(Vector<float>.Count, valueCount - l * Vector<float>.Count) * sizeof(float);

            float[] values1 = new float[Vector<float>.Count];
            float[] values2 = new float[Vector<float>.Count];

            Buffer.BlockCopy(src1, srcOffset1 + l * Vector<float>.Count, values1, 0, length);
            Buffer.BlockCopy(src2, srcOffset2 + l * Vector<float>.Count, values2, 0, length);

            Vector<float> vector1 = new(values1);
            Vector<float> vector2 = new(values2);

            totalVector += vector1 * vector2;
        }

        float totalFloat = 0;
        for (int l = 0; l < Vector<float>.Count; l++)
        {
            totalFloat += totalVector[l];
        }

        return totalFloat;
    }



    public readonly Matrix matrixA;

    public readonly Matrix matrixB;

    public readonly float randomNum;

    public const int SIZE = 100;

    static float[,] RandomMatrix(int size, int range)
    {
        float[,] matrix = new float[size, size];
        Random random = new();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * range - range;
            }
        }
        return matrix;
    }

    public VectorBenchmarks()
    {
        matrixA = new(RandomMatrix(SIZE, 1000));
        matrixB = new(RandomMatrix(SIZE, 1000));

        randomNum = (float)new Random().NextDouble() * 10000f;
    }



    [Benchmark]
    public Matrix DefaultAdd() => matrixA + matrixB;

    [Benchmark]
    public Matrix DefaultScale() => matrixA * randomNum;

    [Benchmark]
    public Matrix DefaultProd() => matrixA * matrixB;



    [Benchmark]
    public Matrix PreVectorAdd() => OperatorAddPre(matrixA, matrixB);

    [Benchmark]
    public Matrix PreVectorScale() => OperatorScalePre(matrixA, randomNum);

    [Benchmark]
    public Matrix PreVectorProd() => OperatorProdPre(matrixA, matrixB);



    [Benchmark]
    public Matrix InVectorAdd() => OperatorAddIn(matrixA, matrixB);

    [Benchmark]
    public Matrix InVectorScale() => OperatorScaleIn(matrixA, randomNum);

    [Benchmark]
    public Matrix InVectorProd() => OperatorProdIn(matrixA, matrixB);

}

/* size: 100 (before fix)
| Method         | Mean         | Error       | StdDev      | Median       |
|--------------- |-------------:|------------:|------------:|-------------:|
| DefaultAdd     |    18.546 us |   0.3669 us |   0.6231 us |    18.481 us |
| DefaultScale   |     8.804 us |   0.1720 us |   0.2520 us |     8.779 us |
| DefaultProd    | 1,377.847 us |  19.3555 us |  16.1627 us | 1,370.131 us |
| PreVectorAdd   |    66.016 us |   1.2697 us |   1.3586 us |    65.636 us |
| PreVectorScale |    63.330 us |   1.2474 us |   1.1668 us |    63.031 us |
| PreVectorProd  |   308.547 us |   6.1158 us |   7.7345 us |   305.625 us |
| InVectorAdd    |    80.565 us |   1.5524 us |   1.3762 us |    81.029 us |
| InVectorScale  |    54.138 us |   0.7499 us |   0.6262 us |    54.043 us |
| InVectorProd   | 5,566.406 us | 155.9426 us | 449.9302 us | 5,323.045 us |
*/

/* size: 500  (before fix)
| Method         | Mean         | Error        | StdDev       | Median       |
|--------------- |-------------:|-------------:|-------------:|-------------:|
| DefaultAdd     |     552.1 us |     10.28 us |      9.62 us |     551.5 us |
| DefaultScale   |     323.2 us |      5.42 us |      5.07 us |     323.3 us |
| DefaultProd    | 165,112.4 us |  2,205.06 us |  1,954.72 us | 165,562.8 us |
| PreVectorAdd   |   3,765.6 us |     90.48 us |    266.78 us |   3,752.8 us |
| PreVectorScale |   3,853.9 us |     76.78 us |    147.93 us |   3,887.7 us |
| PreVectorProd  |  40,841.6 us |  2,334.49 us |  6,883.30 us |  42,256.9 us |
| InVectorAdd    |   3,189.7 us |    220.59 us |    650.41 us |   3,053.7 us |
| InVectorScale  |   2,201.2 us |     56.84 us |    165.81 us |   2,153.4 us |
| InVectorProd   | 617,909.0 us | 10,883.53 us | 13,365.95 us | 616,641.8 us |
*/

/* size: 1000  (before fix)
| Method         | Mean         | Error      | StdDev      |
|--------------- |-------------:|-----------:|------------:|
| DefaultAdd     |     2.326 ms |  0.0336 ms |   0.0314 ms |
| DefaultScale   |     1.456 ms |  0.0238 ms |   0.0211 ms |
| DefaultProd    | 1,311.372 ms | 18.5551 ms |  17.3565 ms |
| PreVectorAdd   |    13.226 ms |  0.2627 ms |   0.3416 ms |
| PreVectorScale |    11.304 ms |  0.1913 ms |   0.2046 ms |
| PreVectorProd  |   221.525 ms |  4.2910 ms |   4.7694 ms |
| InVectorAdd    |     9.502 ms |  0.1874 ms |   0.1753 ms |
| InVectorScale  |     7.118 ms |  0.0517 ms |   0.0403 ms |
| InVectorProd   | 4,907.852 ms | 90.7938 ms | 100.9171 ms |
*/

/* size 100 (after fix)
| Method         | Mean         | Error      | StdDev     |
|--------------- |-------------:|-----------:|-----------:|
| DefaultAdd     |    12.917 us |  0.2220 us |  0.3522 us |
| DefaultScale   |     8.594 us |  0.1205 us |  0.1006 us |
| DefaultProd    | 1,630.031 us | 23.8919 us | 21.1795 us |
| PreVectorAdd   |    66.148 us |  0.8088 us |  0.6754 us |
| PreVectorScale |    63.814 us |  0.6000 us |  0.5010 us |
| PreVectorProd  |   302.989 us |  5.7470 us |  8.7763 us |
| InVectorAdd    |    80.488 us |  1.5358 us |  1.8283 us |
| InVectorScale  |    56.068 us |  1.1016 us |  1.5443 us |
| InVectorProd   | 5,256.522 us | 82.2249 us | 72.8902 us |
*/