
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

    public StructBenchmarking()
    {
        A = RandomMatrix(100, 1000);
        B = RandomMatrix(100, 1000);

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
}
