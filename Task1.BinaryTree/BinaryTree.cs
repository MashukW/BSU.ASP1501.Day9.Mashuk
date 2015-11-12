using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTree
{
    public class BinaryTree<T> : IEnumerable<T>
    {
        private BinaryTreeNode<T> head;
        private Func<T, T, int> comparison;
        private int count;

        public int Count
        {
            get { return count; }
        }

        public IComparer<T> Comparison
        {
            set
            {
                if (value == null)
                    return;

                SetComparison(value);
            }
        }

        #region Constructors
        public BinaryTree()
        {
            if (!typeof(T).GetInterfaces().Contains(typeof(IComparable<T>)))
                throw new ArgumentNullException("Comparator is null");

            SetComparison();
        }

        public BinaryTree(IEnumerable<T> values) : this()
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (T value in values)
                Add(value);
        }

        public BinaryTree(IComparer<T> comparer)
        {
            if (comparer == null)
                if (typeof(T).GetInterfaces().Contains(typeof(IComparable<T>)))
                    SetComparison();
                else
                    throw new ArgumentNullException("Comparator is null");

            else
                SetComparison(comparer);
        }

        public BinaryTree(IEnumerable<T> values, IComparer<T> comparer) : this(comparer)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (T value in values)
                Add(value);
        }
        #endregion

        #region Add a new node to the tree

        public void Add(T value)
        {
            if (head == null)
                head = new BinaryTreeNode<T>(value);
            else
                AddTo(head, value);

            count++;
        }

        private void AddTo(BinaryTreeNode<T> node, T value)
        {
            if (comparison(value, node.Value) < 0)
            {
                if (node.Left == null)
                    node.Left = new BinaryTreeNode<T>(value);
                else
                    AddTo(node.Left, value);
            }

            else
            {
                if (node.Right == null)
                    node.Right = new BinaryTreeNode<T>(value);
                else
                    AddTo(node.Right, value);
            }
        }

        #endregion

        #region Search a node in the tree

        public bool Contains(T value)
        {
            BinaryTreeNode<T> parent;
            return FindWithParent(value, out parent) != null;
        }

        private BinaryTreeNode<T> FindWithParent(T value, out BinaryTreeNode<T> parent)
        {
            BinaryTreeNode<T> current = head;
            parent = null;

            while (current != null)
            {
                int result = comparison(current.Value, value);

                if (result > 0)
                {
                    parent = current;
                    current = current.Left;
                }

                else if (result < 0)
                {
                    parent = current;
                    current = current.Right;
                }

                else
                    break;
            }

            return current;
        }

        #endregion

        #region Remove a node from a tree

        public bool Remove(T value)
        {
            BinaryTreeNode<T> current;
            BinaryTreeNode<T> parent;

            current = FindWithParent(value, out parent);

            if (current == null)
                return false;

            count--;

            if (current.Right == null)
            {
                PermutationNodes(parent, current, current.Left);
            }

            else if (current.Right.Left == null)
            {
                current.Right.Left = current.Left;

                PermutationNodes(parent, current, current.Right);
            }

            else
            {
                BinaryTreeNode<T> leftmost = current.Right.Left;
                BinaryTreeNode<T> leftmostParent = current.Right;

                while (leftmost.Left != null)
                {
                    leftmostParent = leftmost;
                    leftmost = leftmost.Left;
                }

                leftmostParent.Left = leftmost.Right;

                leftmost.Left = current.Left;
                leftmost.Right = current.Right;

                PermutationNodes(parent, current, leftmost);
            }

            return true;
        }

        #endregion

        #region Clear tree

        public void Clear()
        {
            head = null;
            count = 0;
        }

        #endregion

        #region Method GetEnumerator() (IEnumerable<T>)

        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTraversal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Inorder traversal of the tree

        public IEnumerator<T> InOrderTraversal()
        {
            if (head == null)
                throw new InvalidOperationException("Tree is empty");

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = head;

            bool goLeftNext = true;

            stack.Push(current);

            while (stack.Count > 0)
            {
                if (goLeftNext)
                {
                    while (current.Left != null)
                    {
                        stack.Push(current);
                        current = current.Left;
                    }
                }

                yield return current.Value;

                if (current.Right != null)
                {
                    current = current.Right;
                    goLeftNext = true;
                }
                else
                {
                    current = stack.Pop();
                    goLeftNext = false;
                }
            }
        }

        #endregion

        #region Preorder traversal of the tree

        public IEnumerator<T> PreOrderTraversal()
        {
            if (head == null)
                throw new InvalidOperationException("Tree is empty");

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = head;

            bool goLeftNext = true;

            stack.Push(current);

            while (stack.Count > 0)
            {
                if (goLeftNext)
                {
                    while (current.Left != null)
                    {
                        yield return current.Value;
                        if (current.Left != null)
                        {
                            stack.Push(current);
                            current = current.Left;
                        }
                    }
                    yield return current.Value;
                }

                if (current.Right != null)
                {
                    current = current.Right;
                    goLeftNext = true;
                }
                else
                {
                    current = stack.Pop();
                    goLeftNext = false;
                }
            }
        }

        #endregion

        #region Postorder traversal of the tree

        public IEnumerator<T> PostOrder()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void PermutationNodes(BinaryTreeNode<T> parent, BinaryTreeNode<T> currentNode,
            BinaryTreeNode<T> newNode)
        {
            if (parent == null)
                head = newNode;

            else
            {
                int result = comparison(parent.Value, currentNode.Value);
                if (result > 0)
                    parent.Left = newNode;
                else if (result < 0)
                    parent.Right = newNode;
            }

        }

        private void SetComparison(IComparer<T> comparer)
        {
            comparison = ((T value1, T value2) =>
            {
                return comparer.Compare(value1, value2);
            });
        }

        private void SetComparison()
        {
            comparison = ((T value1, T value2) =>
            {
                return (value1 as IComparable<T>).CompareTo(value2);
            });
        }
    }
}