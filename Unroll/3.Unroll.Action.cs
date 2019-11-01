using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metaprogramming.Unroll
{
    [DisassemblyDiagnoser]
    public class UnrollingActionsMethodExample
    {
        public const int Size = 4096 * 4096;

        float[] _floatArray = new float[Size];

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

        private void ExecuteUnrolled<TStep, T>(T[] data, Func<int, T, T> act) 
            where TStep : struct, IValue
        {
            TStep step = default;
            if (step.Value > 8)
                throw new NotImplementedException("The unroller implementation doesnt support chunks bigger than 8");

            for (int i = 0; i < data.Length; i += step.Value)
            {
                // Every access look like this. 
                //      cmp         ebx,ebp
                //      jae         00007FF7EA482632
                //      movsxd      r14,ebx
                //      vmovss      xmm2,dword ptr[rsi + r14 * 4 + 10h]
                //      mov         rax,rdi
                //      mov         rcx,qword ptr[rax + 8]
                //      mov         edx,ebx
                //      call        qword ptr[rax + 18h]  <-- THIS IS BAD!!! 
                //      vmovss      dword ptr[rsi + r14 * 4 + 10h], xmm0
                // Exactly the same as in the other case.
                data[i] = act(i, data[i]);
                if (step.Value == 1) continue;

                data[i + 1] = act(i + 1, data[i + 1]);
                if (step.Value == 2) continue;

                data[i + 2] = act(i + 2, data[i + 2]);
                if (step.Value == 3) continue;

                data[i + 3] = act(i + 3, data[i + 3]);
                if (step.Value == 4) continue;

                data[i + 4] = act(i + 4, data[i + 4]);
                if (step.Value == 5) continue;

                data[i + 5] = act(i + 5, data[i + 5]);
                if (step.Value == 6) continue;

                data[i + 6] = act(i + 6, data[i + 6]);
                if (step.Value == 7) continue;

                data[i + 7] = act(i + 7, data[i + 7]);
            }

            // Here we should deal with non multiple if they happen. For brevity we are not going to do that, and control the call site instead.
        }

        [Benchmark]
        public void Unrolled8()
        {
            ExecuteUnrolled<Numbers.N8, float>(_floatArray, (idx, d) => idx);
        }

        [Benchmark]
        public void Unrolled4()
        {
            ExecuteUnrolled<Numbers.N4, float>(_floatArray, (idx, d) => idx);
        }
    }
}
