using System;
using System.Collections.Generic;

namespace Metaprogramming.Trees
{
    public class Node
    {
        public int Value;
        public Node Left;
        public Node Right;
    }

    public interface ITreeInsertion
    {
        public void Insert(int value);
    }
}