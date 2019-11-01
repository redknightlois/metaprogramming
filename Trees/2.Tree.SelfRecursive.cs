using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metaprogramming.Trees.Stage2
{

    public interface ITraverseStrategy
    {
        void Traverse<TTraverseStrategy>(Node node) where TTraverseStrategy : struct, ITraverseStrategy;
    }    

    public struct InfixStrategy : ITraverseStrategy
    {
        public void Traverse<TTraverseStrategy>(Node node) where TTraverseStrategy : struct, ITraverseStrategy
        {           
            if (node == null)
                return;

            TTraverseStrategy strategy = default;
            strategy.Traverse<TTraverseStrategy>(node.Left);
            strategy.Traverse<TTraverseStrategy>(node.Right);
        }
    }

    public class Tree : ITreeInsertion
    {
        public Node Root { get; private set; }

        public void Insert(int v)
        {
            var node = Insert(Root, v);
            if (Root == null)
                Root = node;
        }

        private Node Insert(Node root, int v)
        {
            if (root == null)
            {
                root = new Node();
                root.Value = v;
            }
            else if (v < root.Value)
            {
                root.Left = Insert(root.Left, v);
            }
            else
            {
                root.Right = Insert(root.Right, v);
            }

            return root;
        }

        public void Traverse(Node root)
        {
            if (root == null)
                return;

            Traverse(root.Left);
            Traverse(root.Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Traverse<TTraverseStrategy>() where TTraverseStrategy : struct, ITraverseStrategy
        {
            TTraverseStrategy strategy = default;
            strategy.Traverse<TTraverseStrategy>(this.Root);
        }
    }
}