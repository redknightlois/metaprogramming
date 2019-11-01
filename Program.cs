using System;
using BenchmarkDotNet.Running;
using Metaprogramming.Matrix.Benchmark;
using Metaprogramming.Matrix.Stage1;
using Metaprogramming.Matrix.Stage2;
using Stage3a = Metaprogramming.Matrix.Stage3a;
using Stage3b = Metaprogramming.Matrix.Stage3b;
using Metaprogramming.Matrix.Stage4;
using Metaprogramming.Unroll;
using Metaprogramming.Trees;
using Metaprogramming.Trees.Benchmark;

namespace Metaprogramming
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var benchmark1 = new MatrixConstantPropagateBenchmark();
            benchmark1.RowFirst();

            var benchmark2 = new Stage3a.MatrixOverheadBenchmark();
            benchmark2.RowFirst();

            var benchmark3 = new Stage3b.MatrixOverheadBenchmark();
            benchmark3.RowFirst();

            var benchmark4 = new MatrixReadonlyBenchmark();
            benchmark4.RowFirst();

            var benchmark5 = new UnrollingWithNoDataMethodExample();
            benchmark5.Unrolled8();
            //benchmark5.BaselineUnrolled8();

            var benchmark6 = new TreesBenchmark();
            benchmark6.OnStartup();
            benchmark6.StrategyTraverse();

            var summary = BenchmarkRunner.Run<TreesBenchmark>();
            //var summary = BenchmarkRunner.Run<UnrollingWithNoDataMethodExample>();
            //var summary = BenchmarkRunner.Run<MatrixBenchmark>();

            // Console.ReadLine();
        }
    }
}
