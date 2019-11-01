using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metaprogramming.Unroll
{

    [DisassemblyDiagnoser]
    public class UnrollingSimpleMethodExample
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

        public interface IUnrollConfiguration<T>
        {
            int Step { get; }
            void Act(int index, ref T data);
        }

        private void ExecuteUnrolled<TUnroller, T>(T[] data) where TUnroller : struct, IUnrollConfiguration<T>
        {
            TUnroller unroller = default;
            if (unroller.Step > 8)
                throw new NotImplementedException("The unroller implementation doesnt support chunks bigger than 8");

            for (int i = 0; i < data.Length; i += unroller.Step)
            {
                // Every access look like this. 
                //      cmp         eax,ecx
                //      jae         00007FF7DF3424CA
                //      movsxd      r8,eax
                //      lea         r8,[rdx+r8*4+10h]  
                //      vxorps      xmm0,xmm0,xmm0  
                //      vcvtsi2ss   xmm0,xmm0,eax  
                //      vmovss      dword ptr[r8], xmm0
                // Exactly the same as in the other case.
                unroller.Act(i, ref data[i]);
                if (unroller.Step == 1) continue;

                unroller.Act(i + 1, ref data[i + 1]);
                if (unroller.Step == 2) continue;

                unroller.Act(i + 2, ref data[i + 2]);
                if (unroller.Step == 3) continue;

                unroller.Act(i + 3, ref data[i + 3]);
                if (unroller.Step == 4) continue;

                unroller.Act(i + 4, ref data[i + 4]);
                if (unroller.Step == 5) continue;

                unroller.Act(i + 5, ref data[i + 5]);
                if (unroller.Step == 6) continue;

                unroller.Act(i + 6, ref data[i + 6]);
                if (unroller.Step == 7) continue;

                unroller.Act(i + 7, ref data[i + 7]);
            }

            // Here we should deal with non multiple if they happen. For brevity we are not going to do that, and control the call site instead.
        }

        public struct UnrollAction8 : IUnrollConfiguration<float>
        {
            public int Step => 8;

            public void Act(int index, ref float data)
            {
                data = index;
            }
        }

        [Benchmark]
        public void Unrolled8()
        {
            ExecuteUnrolled<UnrollAction8, float>(_floatArray);
        }

        public struct UnrollAction4 : IUnrollConfiguration<float>
        {
            public int Step => 4;

            public void Act(int index, ref float data)
            {
                data = index;
            }
        }

        [Benchmark]
        public void Unrolled4()
        {
            ExecuteUnrolled<UnrollAction8, float>(_floatArray);
        }
    }
}
