using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Tokenizers
{
    public class PriorityQueue<T>
    {
        private readonly List<T> _heap = new List<T>();
        private readonly Func<T, T, bool> _comparator;

        public PriorityQueue(Func<T, T, bool> comparator)
        {
            _comparator = comparator;
        }

        public int Size() => _heap.Count;

        public bool IsEmpty() => Size() == 0;

        public T Peek() => _heap[0];

        public void Push(T value)
        {
            _heap.Add(value);
            SiftUp();
        }

        public T Pop()
        {
            var poppedValue = Peek();
            var bottom = Size() - 1;
            if (bottom > 0)
            {
                Swap(0, bottom);
            }
            _heap.RemoveAt(bottom);
            SiftDown();
            return poppedValue;
        }

        public T Replace(T value)
        {
            var replacedValue = Peek();
            _heap[0] = value;
            SiftDown();
            return replacedValue;
        }

        private int Parent(int i) => (i - 1) / 2;

        private int Left(int i) => 2 * i + 1;

        private int Right(int i) => 2 * i + 2;

        private bool Greater(int i, int j) => _comparator.Invoke(_heap[i], _heap[j]);

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }

        private void SiftUp()
        {
            var node = Size() - 1;
            while (node > 0 && Greater(node, Parent(node)))
            {
                Swap(node, Parent(node));
                node = Parent(node);
            }
        }

        private void SiftDown()
        {
            var node = 0;
            while (true)
            {
                var left = Left(node);
                var right = Right(node);
                var largest = node;
                if (left < Size() && Greater(left, node)) largest = left;
                if (right < Size() && Greater(right, largest)) largest = right;
                if (largest == node) break;
                Swap(node, largest);
                node = largest;
            }
        }
    }
}