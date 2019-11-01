using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Metaprogramming.Matrix.Stage2
{
    public struct RowFirst<TSize2, T> : IStorageLayout<TSize2, T>
        where TSize2 : struct, ISize2
    {
        private TSize2 _config;
        private T[] _storage;

        public void Initialize()
        {
            _config = default;
            _storage = new T[_config.X * _config.Y];
        }

        public void Set(int x, int y, T value)
        {
            _storage[x * _config.Y + y] = value;
        }

        public T Get(int x, int y)
        {
            return _storage[x * _config.Y + y];
        }
    }

    public sealed class Matrix<TStorage, TSize2, T>
        where TStorage : struct, IStorageLayout<TSize2, T>
        where TSize2 : struct, ISize2
    {
        private TStorage _storage;

        public Matrix()
        {
            _storage.Initialize();
        }

        public T this[int x, int y]
        {
            get { return _storage.Get(x, y); }
            set { _storage.Set(x, y, value); }
        }
    }


    [DisassemblyDiagnoser]
    public class MatrixConstantPropagateBenchmark
    {
        public const int Size = 4096;

        struct SizeTuple : ISize2
        {
            public int X { get { return Size; } }
            public int Y { get { return Size; } }
        }

        float[] _floatArray = new float[Size * Size];
        Matrix<RowFirst<SizeTuple, float>, SizeTuple, float> _rowMatrix = new Matrix<RowFirst<SizeTuple, float>, SizeTuple, float>();


        [Benchmark]
        public void NakedArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    // JIT will move the reference for _floatArray into the outer loop 
                    // _floatArray[x * Size + y] = x + y;
                    //      mov         r10,r8
                    _floatArray[x * Size + y] = x + y;
                }
        }

        [Benchmark]
        public void RowFirst()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    // This is the cost of accessing ._storage because the JIT doesnt recognize it can store the reference in a register
                    // _rowMatrix[x, y] = x + y;                    
                    //      mov     r10d,dword ptr[rax]
                    //      mov     r10,r8
                    //      mov     r11,qword ptr[r10]
                    //      cmp     dword ptr[r10],r10d   <-- NULL CHECK :)
                    // If it would, we would get to match the naked access.
                    _rowMatrix[x, y] = x + y;
                }
        }
    }
}