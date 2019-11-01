using System;

namespace Metaprogramming.Unroll
{
    public interface IValue
    {
        int Value { get; }
    }

    public static class Numbers
    {
        public struct N4 : IValue { int IValue.Value => 4; }
        public struct N8 : IValue { int IValue.Value => 8; }
        public struct N16 : IValue { int IValue.Value => 16; }
        public struct N32 : IValue { int IValue.Value => 32; }
    }
}