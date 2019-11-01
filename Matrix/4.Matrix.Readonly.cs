using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Metaprogramming.Matrix.Stage4
{
    public struct RowFirst<TSize2, T> : IStorageLayoutCreate<TSize2, T>
        where TSize2 : struct, ISize2
    {
        private readonly T[] _storage;

        public RowFirst(TSize2 config)
        {
            _storage = new T[config.X * config.Y];
        }

        public TStorage Create<TStorage>() 
            where TStorage : IStorageLayoutCreate<TSize2, T>
        {
            // The struct will create itself but because TStorage is 'generic' there is no way we can return
            // the value. However, there is a known trick that will allow us to bypass the checks.
            var instance = new RowFirst<TSize2, T>(default);

            // We would perform a double cast. That cast will get erased by the JIT compiler because we are
            // dealing with structs. In the normal case, we would get a boxing and unboxing operation
            // but we are dealing with special codepaths that allow us to do this with 0 overhead.
            return (TStorage)(IStorageLayoutCreate<TSize2, T>)instance;

            // There are other versions that also work like:
            // return (TStorage)(object)instance;
            // But for purposes of readability we better constrain to the proper type. 
        }

        public void Set(int x, int y, T value)
        {
            TSize2 config = default;
            _storage[x * config.Y + y] = value;
        }

        public T Get(int x, int y)
        {
            TSize2 config = default;
            return _storage[x * config.Y + y];
        }
    }

    public sealed class Matrix<TStorage, TSize2, T>
        where TStorage : struct, IStorageLayoutCreate<TSize2, T>
        where TSize2 : struct, ISize2
    {
        private readonly TStorage _storage;

        public Matrix()
        {
            // This allows us to do custom type creation without reflection.
            // The devil is in the details. See the implementation of 'Create'
            _storage = _storage.Create<TStorage>();
        }

        public T this[int x, int y]
        {
            get { return _storage.Get(x, y); }
            set { _storage.Set(x, y, value); }
        }
    }


    [DisassemblyDiagnoser]
    public class MatrixReadonlyBenchmark
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
                    _floatArray[x * Size + y] = x + y;
        }

        [Benchmark]
        public void RowFirst()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    // We moved from:
                    //      mov     r10d,dword ptr[rax]
                    //      mov     r10,r8
                    //      mov     r11,qword ptr[r10]
                    //      cmp     dword ptr[r10],r10d   <-- NULL CHECK :)
                    // back to:
                    //      mov     r10,r8
                    //      mov     r10,qword ptr[r10 + 8]
                    //      lea     r11d,[r9+rdx]  
                    //      cmp     r11d,dword ptr[r10 + 8]   <-- NULL CHECK :)
                    // which is, for all uses and purposes, the same, just slightly faster.
                    _rowMatrix[x, y] = x + y;
                }                
        }
    }
}