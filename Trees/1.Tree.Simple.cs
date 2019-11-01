using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metaprogramming.Trees.Stage1
{

    public interface ITraverseStrategy
    {
        IEnumerable<Node> Enumerate(Node node);
    }

    public struct InfixStrategy : ITraverseStrategy
    {
        public IEnumerable<Node> Enumerate(Node root)
        {
            if (root == null)
                yield break;

            yield return root.Left;
            yield return root.Right;
        }
    }

    public struct PostfixStrategy : ITraverseStrategy
    {
        public IEnumerable<Node> Enumerate(Node root)
        {
            if (root == null)
                yield break;

            yield return root.Right;
            yield return root.Left;
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

        public void Traverse<TTraverseStrategy>() where TTraverseStrategy : struct, ITraverseStrategy
        {
            Traverse<TTraverseStrategy>(this.Root);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Traverse<TTraverseStrategy>(Node node) where TTraverseStrategy : struct, ITraverseStrategy
        {
            TTraverseStrategy strategy = default;
            foreach (var n in strategy.Enumerate(node))
                Traverse<TTraverseStrategy>(n);
        }
    }
}