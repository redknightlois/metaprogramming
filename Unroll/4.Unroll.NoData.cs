using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metaprogramming.Unroll
{
    [DisassemblyDiagnoser]
    public class UnrollingWithNoDataMethodExample
    {
        public const int Size = 4096 * 4096;

        float[] _floatArray = new float[Size];

        [Benchmark]
        public void Baseline()
        {
            for (int i = 0; i < _floatArray.Length; i++)
            {
                _floatArray[i] = i;
            }
        }

        [Benchmark]
        public void BaselineUnrolled8()
        {
            for (int i = 0; i < _floatArray.Length; i += 8)
            {
                // Each one of the accesses look like this... 
                //     cmp         eax,r8d
                //     jae         00007FF7DF332623
                //     movsxd      r9,eax
                //     vxorps      xmm0,xmm0,xmm0
                //     vcvtsi2ss   xmm0,xmm0,eax
                //     vmovss      dword ptr[rcx + r9 * 4 + 10h], xmm0
                _floatArray[i] = i;

                _floatArray[i + 1] = i + 1;
                _floatArray[i + 2] = i + 2;
                _floatArray[i + 3] = i + 3;
                _floatArray[i + 4] = i + 4;
                _floatArray[i + 5] = i + 5;
                _floatArray[i + 6] = i + 6;
                _floatArray[i + 7] = i + 7;
            }

            // Here we should deal with non multiple if they happen. For brevity we are not going to do that, and control the call site instead.
        }

        public interface IUnrollContext<TStep, T> where TStep : struct, IValue
        {
            void Reset();
            bool Advance();
            void Act(int idx);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteUnrolled<TUnrollContext, TStep, T>(TUnrollContext context)            
            where TUnrollContext : struct, IUnrollContext<TStep, T>
            where TStep : struct, IValue
        {
            TStep step = default;
            if (step.Value > 8)
                throw new NotImplementedException("The unroller implementation doesnt support chunks bigger than 8");
            
            while (context.Advance())
            {
                context.Act(0);
                if (step.Value == 1) continue;

                context.Act(1);
                if (step.Value == 2) continue;

                context.Act(2);
                if (step.Value == 3) continue;

                context.Act(3);
                if (step.Value == 4) continue;

                context.Act(4);
                if (step.Value == 5) continue;

                context.Act(5);
                if (step.Value == 6) continue;

                context.Act(6);
                if (step.Value == 7) continue;

                context.Act(7);
            }            

            // Here we should deal with non multiple if they happen. For brevity we are not going to do that, and control the call site instead.
        }

        public struct UnrollContext<TStep> : IUnrollContext<TStep, float>
            where TStep : struct, IValue
        {
            private int position;
            private float[] data;

            public UnrollContext(float[] data)
            {
                TStep step = default;
                this.position = -step.Value;
                this.data = data;
            }

            public void Reset()
            {
                TStep step = default;
                this.position = -step.Value;
            }

            public bool Advance()
            {
                TStep step = default;
                this.position += step.Value;
                return this.position + step.Value < this.data.Length;
            }

            public void Act(int index)
            {
                this.data[position + index] = position + index;
            }
        }

        [Benchmark]
        public void Unrolled8()
        {            
            // The use of 'default' numbers is something that could easily be included in the language and would make a lot of generic methods easier to write.            
            ExecuteUnrolled<UnrollContext<Numbers.N8>, Numbers.N8, float>(new UnrollContext<Numbers.N8>(_floatArray));
        }

        [Benchmark]
        public void Unrolled4()
        {
            ExecuteUnrolled<UnrollContext<Numbers.N4>, Numbers.N4, float>(new UnrollContext<Numbers.N4>(_floatArray));
        }
    }
}
