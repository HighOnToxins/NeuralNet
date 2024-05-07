
using BenchmarkDotNet.Attributes;

namespace Benchmarking;

public class StructBenchmarking
{

    public readonly struct ReadonlyMatrix 
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
        }

        public ReadonlyMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private ReadonlyMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private ReadonlyMatrix(ReadonlyMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        public static ReadonlyMatrix operator +(ReadonlyMatrix A, ReadonlyMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            float[] C = new float[A.Width * A.Height];

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i * A.Height + j] = A[i, j] + B[i, j];
                }
            }

            return new ReadonlyMatrix(A.Height, A.Width, C);
        }

        public static ReadonlyMatrix operator *(ReadonlyMatrix A, ReadonlyMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            float[] B2 = new float[B.Height * B.Width];

            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i * B.Width + j] = B[j, i];
                }
            }

            float[] C = new float[A.Height * B.Width];

            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j * B.Height + k];
                    }

                    C[i * B.Width + j] = total;
                }
            }

            return new ReadonlyMatrix(A.Height, B.Width, C);
        }

        public static ReadonlyMatrix NonTransProduct(ReadonlyMatrix A, ReadonlyMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            float[] C = new float[A.Height * B.Width];
            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B[k, j];
                    }

                    C[i * B.Width + j] = total;
                }
            }

            return new ReadonlyMatrix(A.Height, B.Width, C);
        }

    }

    public readonly struct WriteableMatrix 
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            set
            {
                values[row * Width + col] = value;
            }
        }

        public WriteableMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private WriteableMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private WriteableMatrix(WriteableMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        private WriteableMatrix(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Width * Height];
        }

        public static WriteableMatrix operator +(WriteableMatrix A, WriteableMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            WriteableMatrix C = new(A);

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }

            return C;
        }

        public static WriteableMatrix operator *(WriteableMatrix A, WriteableMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            WriteableMatrix B2 = new(B.Height, B.Width);
            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i, j] = B[j, i];
                }
            }

            WriteableMatrix C = new(A.Height, B.Width);
            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j, k];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

    }

    public readonly struct ReadonlyMatrixPrivSet
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            private set
            {
                values[row * Width + col] = value;
            }
        }

        public ReadonlyMatrixPrivSet(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private ReadonlyMatrixPrivSet(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private ReadonlyMatrixPrivSet(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Width * Height];
        }

        private ReadonlyMatrixPrivSet(ReadonlyMatrixPrivSet matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        public static ReadonlyMatrixPrivSet operator +(ReadonlyMatrixPrivSet A, ReadonlyMatrixPrivSet B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            float[] C = new float[A.Width * A.Height];

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i * A.Height + j] = A[i, j] + B[i, j];
                }
            }

            return new ReadonlyMatrixPrivSet(A.Height, A.Width, C);
        }

        public static ReadonlyMatrixPrivSet operator *(ReadonlyMatrixPrivSet A, ReadonlyMatrixPrivSet B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            float[] B2 = new float[B.Height * B.Width];

            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i * B.Width + j] = B[j, i];
                }
            }

            ReadonlyMatrixPrivSet C = new(A.Height, B.Width);

            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j * B.Height + k];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

    }

    public class ReadonlyClassMatrix
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
        }

        public ReadonlyClassMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private ReadonlyClassMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private ReadonlyClassMatrix(ReadonlyClassMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        public static ReadonlyClassMatrix operator +(ReadonlyClassMatrix A, ReadonlyClassMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            float[] C = new float[A.Width * A.Height];

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i * A.Height + j] = A[i, j] + B[i, j];
                }
            }

            return new ReadonlyClassMatrix(A.Height, A.Width, C);
        }

        public static ReadonlyClassMatrix operator *(ReadonlyClassMatrix A, ReadonlyClassMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            float[] B2 = new float[B.Height * B.Width];

            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i * B.Width + j] = B[j, i];
                }
            }

            float[] C = new float[A.Height * B.Width];

            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j * B.Height + k];
                    }

                    C[i * B.Width + j] = total;
                }
            }

            return new ReadonlyClassMatrix(A.Height, B.Width, C);
        }

    }

    public class WriteableClassMatrix
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            set
            {
                values[row * Width + col] = value;
            }
        }

        public WriteableClassMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private WriteableClassMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private WriteableClassMatrix(WriteableClassMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        private WriteableClassMatrix(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Width * Height];
        }

        public static WriteableClassMatrix operator +(WriteableClassMatrix A, WriteableClassMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            WriteableClassMatrix C = new(A);

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }

            return C;
        }

        public static WriteableClassMatrix operator *(WriteableClassMatrix A, WriteableClassMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            WriteableClassMatrix B2 = new(B.Height, B.Width);
            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i, j] = B[j, i];
                }
            }

            WriteableClassMatrix C = new(A.Height, B.Width);
            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j, k];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

    }

    public class ReadonlyPrivSetClassMatrix
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            private set
            {
                values[row * Width + col] = value;
            }
        }

        public ReadonlyPrivSetClassMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private ReadonlyPrivSetClassMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private ReadonlyPrivSetClassMatrix(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Height * Width];
        }

        private ReadonlyPrivSetClassMatrix(ReadonlyPrivSetClassMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        public static ReadonlyPrivSetClassMatrix operator +(ReadonlyPrivSetClassMatrix A, ReadonlyPrivSetClassMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            float[] C = new float[A.Width * A.Height];

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i * A.Height + j] = A[i, j] + B[i, j];
                }
            }

            return new ReadonlyPrivSetClassMatrix(A.Height, A.Width, C);
        }

        public static ReadonlyPrivSetClassMatrix operator *(ReadonlyPrivSetClassMatrix A, ReadonlyPrivSetClassMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            ReadonlyPrivSetClassMatrix B2 = new(B.Height, B.Width);
            for(int i = 0; i < B.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    B2[i, j] = B[j, i];
                }
            }

            ReadonlyPrivSetClassMatrix C = new(A.Height, B.Width);

            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B2[j, k];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

    }

    public class NonTransposeClassMatrix
    {
        private readonly float[] values;

        public int Height { get; }
        public int Width { get; }

        public float this[int row, int col]
        {
            get
            {
                return values[row * Width + col];
            }
            private set
            {
                values[row * Width + col] = value;
            }
        }

        public NonTransposeClassMatrix(float[,] matrix)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix, 0, values, 0, values.Length * sizeof(float));
        }

        private NonTransposeClassMatrix(int height, int width, float[] values)
        {
            Height = height;
            Width = width;
            this.values = values;
        }

        private NonTransposeClassMatrix(int height, int width)
        {
            Height = height;
            Width = width;
            values = new float[Height * Width];
        }

        private NonTransposeClassMatrix(NonTransposeClassMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;

            values = new float[Width * Height];
            Buffer.BlockCopy(matrix.values, 0, values, 0, values.Length * sizeof(float));
        }

        public static NonTransposeClassMatrix operator +(NonTransposeClassMatrix A, NonTransposeClassMatrix B)
        {
            if(A.Width != B.Width || A.Height != B.Height)
            {
                throw new ArgumentException("Expected matrices with equal lengthed sides!");
            }

            float[] C = new float[A.Width * A.Height];

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Height; j++)
                {
                    C[i * A.Height + j] = A[i, j] + B[i, j];
                }
            }

            return new NonTransposeClassMatrix(A.Height, A.Width, C);
        }

        public static NonTransposeClassMatrix operator *(NonTransposeClassMatrix A, NonTransposeClassMatrix B)
        {
            if(A.Width != B.Height)
            {
                throw new ArgumentException("Expected matrices such that the second length of " +
                    "the first matrix was equal to the first length of the second!");
            }

            NonTransposeClassMatrix C = new(A.Height, B.Width);
            for(int i = 0; i < A.Height; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    float total = 0;

                    for(int k = 0; k < A.Width; k++)
                    {
                        total += A[i, k] * B[k, j];
                    }

                    C[i, j] = total;
                }
            }

            return C;
        }

    }

    public static float[,] Transpose(float[,] A)
    {
        float[,] B = new float[A.GetLength(1), A.GetLength(0)];

        for(int i = 0; i < B.GetLength(0); i++)
        {
            for(int j = 0; j < B.GetLength(1); j++)
            {
                B[i, j] = A[j, i];
            }
        }

        return B;
    }

    public static float[,] Product(float[,] A, float[,] B)
    {
        if(A.GetLength(1) != B.GetLength(0))
        {
            throw new ArgumentException("Expected matrices such that the second length of " +
                "the first matrix was equal to the first length of the second!");
        }

        float[,] B2 = Transpose(B);
        float[,] C = new float[A.GetLength(0), B.GetLength(1)];

        for(int i = 0; i < C.GetLength(0); i++)
        {
            for(int j = 0; j < C.GetLength(1); j++)
            {
                float total = 0;

                for(int k = 0; k < A.GetLength(1); k++)
                {
                    total += A[i, k] * B2[j, k];
                }

                C[i, j] = total;
            }
        }

        return C;
    }


    private readonly float[,] A;
    private readonly float[,] B;

    private readonly ReadonlyMatrix readonlyA;
    private readonly ReadonlyMatrix readonlyB;

    private readonly WriteableMatrix writeableA;
    private readonly WriteableMatrix writeableB;

    private readonly ReadonlyMatrixPrivSet readonlyPrivSetA;
    private readonly ReadonlyMatrixPrivSet readonlyPrivSetB;

    private readonly ReadonlyClassMatrix readonlyClassA;
    private readonly ReadonlyClassMatrix readonlyClassB;

    private readonly WriteableClassMatrix writeableClassA;
    private readonly WriteableClassMatrix writeableClassB;

    private readonly ReadonlyPrivSetClassMatrix readonlyPrivSetClassA;
    private readonly ReadonlyPrivSetClassMatrix readonlyPrivSetClassB;

    private readonly NonTransposeClassMatrix nonTransA;
    private readonly NonTransposeClassMatrix nonTransB;

    private const int SIZE = 2_000;

    public StructBenchmarking()
    {
        A = RandomMatrix(SIZE, 1000);
        B = RandomMatrix(SIZE, 1000);

        readonlyA = new(A);
        readonlyB = new(B);

        writeableA = new(A);
        writeableB = new(B);

        readonlyPrivSetA = new(A);
        readonlyPrivSetB = new(B);

        readonlyClassA = new(A);
        readonlyClassB = new(B);

        writeableClassA = new(A);
        writeableClassB = new(B);

        readonlyPrivSetClassA = new(A);
        readonlyPrivSetClassB = new(B);

        nonTransA = new(A);
        nonTransB = new(B);
    }

    static float[,] RandomMatrix(int size, int range)
    {
        float[,] matrix = new float[size, size];
        Random random = new();
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                matrix[i, j] = (float)random.NextDouble() * 2 * range - range;
            }
        }
        return matrix;
    }

    [Benchmark]
    public float[,] DoubleArrayProduct() => Product(A, B);

    [Benchmark]
    public ReadonlyMatrix ReadonlyProduct() => readonlyA * readonlyB;

    [Benchmark]
    public WriteableMatrix WriteableProduct() => writeableA * writeableB;

    [Benchmark]
    public ReadonlyMatrixPrivSet PrivateSetProduct() => readonlyPrivSetA * readonlyPrivSetB;

    [Benchmark]
    public ReadonlyClassMatrix ReadonlyClassProduct() => readonlyClassA * readonlyClassB;

    [Benchmark]
    public WriteableClassMatrix WriteableClassProduct() => writeableClassA * writeableClassB;

    [Benchmark]
    public ReadonlyPrivSetClassMatrix ReadonlyPrivSetClassProduct() => readonlyPrivSetClassA * readonlyPrivSetClassB;

    [Benchmark]
    public NonTransposeClassMatrix NonTransposeClassProduct() => nonTransA * nonTransB;

    [Benchmark]
    public ReadonlyMatrix NonTransposeStructProduct() => ReadonlyMatrix.NonTransProduct(readonlyA, readonlyB);
}

/* size: 100
| Method                      | Mean     | Error     | StdDev    |
|---------------------------- |---------:|----------:|----------:|
| DoubleArrayProduct          | 3.117 ms | 0.0622 ms | 0.1039 ms |
| ReadonlyProduct             | 1.152 ms | 0.0227 ms | 0.0303 ms | *
| WriteableProduct            | 1.395 ms | 0.0275 ms | 0.0230 ms |
| PrivateSetProduct           | 1.164 ms | 0.0215 ms | 0.0410 ms |
| ReadonlyClassProduct        | 1.367 ms | 0.0077 ms | 0.0072 ms |
| WriteableClassProduct       | 1.132 ms | 0.0220 ms | 0.0195 ms | *
| ReadonlyPrivSetClassProduct | 1.135 ms | 0.0226 ms | 0.0212 ms | *
*/

/* size: 100
| Method                      | Mean     | Error     | StdDev    |
|---------------------------- |---------:|----------:|----------:|
| DoubleArrayProduct          | 2.998 ms | 0.0560 ms | 0.0524 ms |
| ReadonlyProduct             | 1.110 ms | 0.0062 ms | 0.0049 ms | *
| WriteableProduct            | 1.369 ms | 0.0230 ms | 0.0215 ms |
| PrivateSetProduct           | 1.119 ms | 0.0157 ms | 0.0147 ms | *
| ReadonlyClassProduct        | 1.378 ms | 0.0134 ms | 0.0125 ms |
| WriteableClassProduct       | 1.119 ms | 0.0056 ms | 0.0046 ms | *
| ReadonlyPrivSetClassProduct | 1.118 ms | 0.0084 ms | 0.0078 ms | * 
| NonTransposeClassProduct    | 1.105 ms | 0.0203 ms | 0.0180 ms | *
*/

/* size: 100
| Method                      | Mean     | Error     | StdDev    |
|---------------------------- |---------:|----------:|----------:|
| DoubleArrayProduct          | 3.027 ms | 0.0603 ms | 0.0805 ms |
| ReadonlyProduct             | 1.128 ms | 0.0209 ms | 0.0195 ms | *
| WriteableProduct            | 1.366 ms | 0.0136 ms | 0.0128 ms |
| PrivateSetProduct           | 1.119 ms | 0.0213 ms | 0.0228 ms | *
| ReadonlyClassProduct        | 1.373 ms | 0.0147 ms | 0.0137 ms |
| WriteableClassProduct       | 1.117 ms | 0.0100 ms | 0.0088 ms | *
| ReadonlyPrivSetClassProduct | 1.116 ms | 0.0074 ms | 0.0069 ms | *
| NonTransposeClassProduct    | 1.088 ms | 0.0057 ms | 0.0053 ms | *
| NonTransposeStructProduct   | 1.346 ms | 0.0040 ms | 0.0036 ms |
*/

/* size: 1_000
| Method                      | Mean    | Error    | StdDev   |
|---------------------------- |--------:|---------:|---------:|
| DoubleArrayProduct          | 2.931 s | 0.0434 s | 0.0406 s |
| ReadonlyProduct             | 1.051 s | 0.0063 s | 0.0059 s | *
| WriteableProduct            | 1.300 s | 0.0083 s | 0.0078 s |
| PrivateSetProduct           | 1.055 s | 0.0120 s | 0.0112 s | *
| ReadonlyClassProduct        | 1.298 s | 0.0040 s | 0.0033 s |
| WriteableClassProduct       | 1.058 s | 0.0071 s | 0.0059 s | *
| ReadonlyPrivSetClassProduct | 1.061 s | 0.0103 s | 0.0086 s | *
| NonTransposeClassProduct    | 1.177 s | 0.0051 s | 0.0040 s |
| NonTransposeStructProduct   | 1.352 s | 0.0118 s | 0.0099 s |
*/

/* size: 2_000
| Method                      | Mean     | Error    | StdDev   |
|---------------------------- |---------:|---------:|---------:|
| DoubleArrayProduct          | 23.518 s | 0.0805 s | 0.0753 s |
| ReadonlyProduct             |  8.887 s | 0.0757 s | 0.0708 s | *
| WriteableProduct            | 10.622 s | 0.0166 s | 0.0155 s |
| PrivateSetProduct           |  8.810 s | 0.0258 s | 0.0216 s | *
| ReadonlyClassProduct        | 10.674 s | 0.0123 s | 0.0115 s |
| WriteableClassProduct       |  8.891 s | 0.0139 s | 0.0130 s | *
| ReadonlyPrivSetClassProduct |  8.879 s | 0.0085 s | 0.0071 s | *
| NonTransposeClassProduct    | 25.787 s | 0.0743 s | 0.0659 s |
| NonTransposeStructProduct   | 25.018 s | 0.1033 s | 0.0806 s |
*/