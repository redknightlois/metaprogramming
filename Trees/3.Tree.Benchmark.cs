using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Stage1 = Metaprogramming.Trees.Stage1;
using Stage2 = Metaprogramming.Trees.Stage2;

namespace Metaprogramming.Trees.Benchmark
{
    [DisassemblyDiagnoser]
    public class TreesBenchmark
    {
        public const int Size = 4096;

        private Stage1.Tree _tree1 = new Stage1.Tree();
        private Stage2.Tree _tree2 = new Stage2.Tree();

        [GlobalSetup]
        public void OnStartup()
        {
            var values = new int[Size];
            for (int i = 0; i < Size; i++)
                values[i] = i;

            var generator = new Random(1337); // We want reproducible results. 

            // Shuffling to avoid the worst case scenario in the tree insertion. 
            for (int i = 0; i < Size; i++)
            {
                int x = generator.Next(0, Size);
                int y = generator.Next(0, Size);

                int aux = values[x];
                values[y] = values[x];
                values[x] = aux;
            }

            // Insert on the actual trees.
            for (int i = 0; i < Size; i++)
            {
                _tree1.Insert(values[i]);
                _tree2.Insert(values[i]);
            }
        }

        [Benchmark]
        public void BaselineTraverse()
        {
            _tree1.Traverse(_tree1.Root);
        }

        [Benchmark]
        public void YieldTraverse()
        {
            _tree1.Traverse<Stage1.InfixStrategy>(_tree1.Root);
        }

        [Benchmark]
        public void StrategyTraverse()
        {
            _tree2.Traverse<Stage2.InfixStrategy>();
        }

    }
}