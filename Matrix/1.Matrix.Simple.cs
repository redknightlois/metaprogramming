using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Metaprogramming.Matrix.Stage1
{
    public interface IStorageLayout<T>
    {
        void Initialize(int x, int y);
        void Set(int x, int y, T value);
        T Get(int x, int y);
    }

    public struct RowFirst<T> : IStorageLayout<T>
    {
        private int _ySize;
        private T[] _storage;

        public void Initialize(int xSize, int ySize)
        {
            _ySize = ySize;
            _storage = new T[xSize * ySize];
        }

        public void Set(int x, int y, T value)
        {
            int idx = x * _ySize + y;
            _storage[idx] = value;
        }

        public T Get(int x, int y)
        {
            int idx = x * _ySize + y;
            return _storage[idx];
        }
    }

    public struct ColumnFirst<T> : IStorageLayout<T>
    {
        private int _xSize;
        private T[] _storage;

        public void Initialize(int xSize, int ySize)
        {
            _xSize = xSize;
            _storage = new T[xSize * ySize];
        }

        public void Set(int x, int y, T value)
        {
            int idx = y * _xSize + x;
            _storage[idx] = value;
        }

        public T Get(int x, int y)
        {
            int idx = y * _xSize + x;
            return _storage[idx];
        }
    }

    public class Matrix<TStorage, T> where TStorage : struct, IStorageLayout<T>
    {
        TStorage _storage;

        public Matrix(int xSize, int ySize)
        {
            _storage.Initialize(xSize, ySize);
        }

        public T this[int x, int y]
        {
            get { return _storage.Get(x, y); }
            set { _storage.Set(x, y, value); }
        }
    }

    [DisassemblyDiagnoser]
    public class MatrixSimpleBenchmark
    {
        public const int Size = 4096;

        Matrix<RowFirst<float>, float> _rowMatrix = new Matrix<RowFirst<float>, float>(Size, Size);
        Matrix<ColumnFirst<float>, float> _columnMatrix = new Matrix<ColumnFirst<float>, float>(Size, Size);
        float[] _floatArray = new float[Size * Size];

        [Benchmark]
        public void NakedRowArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _floatArray[x * Size + y] = x + y;
        }

        [Benchmark]
        public void NakedColumnArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _floatArray[y * Size + x] = x + y;
        }

        [Benchmark]
        public void StorageColumn()
        {            
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _columnMatrix[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix[x, y] = x + y;
        }
    }
}