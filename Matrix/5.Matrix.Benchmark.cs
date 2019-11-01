using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Stage1 = Metaprogramming.Matrix.Stage1;
using Stage2 = Metaprogramming.Matrix.Stage2;
using Stage3a = Metaprogramming.Matrix.Stage3a;
using Stage3b = Metaprogramming.Matrix.Stage3b;
using Stage4 = Metaprogramming.Matrix.Stage4;


namespace Metaprogramming.Matrix.Benchmark
{
    [DisassemblyDiagnoser]
    public class MatrixBenchmark
    {
        public const int Size = 4096;

        struct SizeTuple : ISize2
        {
            public int X { get { return Size; } }
            public int Y { get { return Size; } }
        }

        float[] _floatArray = new float[Size * Size];
        float[][] _floatJaggedArray;
        float[,] _floatMultiArray = new float[Size, Size];

        Stage1.Matrix<Stage1.RowFirst<float>, float> _rowMatrix1 = new Stage1.Matrix<Stage1.RowFirst<float>, float>(Size, Size);
        Stage1.Matrix<Stage1.ColumnFirst<float>, float> _columnMatrix1 = new Stage1.Matrix<Stage1.ColumnFirst<float>, float>(Size, Size);

        Stage2.Matrix<Stage2.RowFirst<SizeTuple, float>, SizeTuple, float> _rowMatrix2 = new Stage2.Matrix<Stage2.RowFirst<SizeTuple, float>, SizeTuple, float>();
        Stage3a.Matrix<Stage3a.RowFirst<SizeTuple, float>, SizeTuple, float> _rowMatrix3a = new Stage3a.Matrix<Stage3a.RowFirst<SizeTuple, float>, SizeTuple, float>();
        Stage3b.Matrix<Stage3b.RowFirst<SizeTuple, float>, SizeTuple, float> _rowMatrix3b = new Stage3b.Matrix<Stage3b.RowFirst<SizeTuple, float>, SizeTuple, float>();        
        Stage4.Matrix<Stage4.RowFirst<SizeTuple, float>, SizeTuple, float> _rowMatrix4 = new Stage4.Matrix<Stage4.RowFirst<SizeTuple, float>, SizeTuple, float>();



        [GlobalSetup]
        public void OnStartup()
        {
            _floatJaggedArray = new float[Size][];
            for (int i = 0; i < Size; i++)
                _floatJaggedArray[i] = new float[Size];
        }

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
        public void NakedMultiArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _floatMultiArray[x, y] = x + y;
        }

        [Benchmark]
        public void NakedJaggedRowArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _floatJaggedArray[x][y] = x + y;
        }

        [Benchmark]
        public void NakedJaggedColumnArray()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _floatJaggedArray[y][x] = x + y;
        }

        [Benchmark]
        public void StorageColumn1()
        {            
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _columnMatrix1[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow1()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix1[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow2()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix2[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow3a()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix3a[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow3b()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix3b[x, y] = x + y;
        }

        [Benchmark]
        public void StorageRow4()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    _rowMatrix4[x, y] = x + y;
        }
    }
}