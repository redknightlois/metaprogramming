using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Metaprogramming.Matrix.Stage3a
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
        private readonly TStorage _storage;

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
    public class MatrixOverheadBenchmark
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
                    // Now the cost has skyrocketted. 
                    // _rowMatrix[x, y] = x + y;                    
                    //  00007FFBB3FC1A23  mov         r10,r8  
                    //      add         r10,8
                    //      vmovdqu     xmm0,xmmword ptr[r10]
                    //      vmovdqu     xmmword ptr[rsp + 20h], xmm0
                    //      mov         r10,qword ptr[rsp + 20h]
                    //      cmp         dword ptr [r10],r10d   <-- NULL CHECK :)
                    // Mhhh. What happened here? Just a readonly? It looks like an struct copy.
                    _rowMatrix[x, y] = x + y;
                }
        }
    }
}