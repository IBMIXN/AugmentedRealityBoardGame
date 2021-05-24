using System;

namespace TbsFramework.Pathfinding.DataStructs
{
    /// <summary>
    /// Prioritized Queue
    /// </summary>
    public interface IPriorityQueue<T>
    {
        /// <summary>
        /// Number of items in the queue, enqueue and returns item with lowest priority value.
        /// </summary>
        int Count { get; }
        void Enqueue(T item, float priority);
        T Dequeue();
    }

    /// <summary>
    /// Node in a priority queue.
    /// </summary>
    class PriorityQueueNode<T> : IComparable
    {
        public T Item { get; private set; }
        public float Priority { get; private set; }

        public PriorityQueueNode(T item, float priority)
        {
            Item = item;
            Priority = priority;
        }

        public int CompareTo(object obj)
        {
            return Priority.CompareTo((obj as PriorityQueueNode<T>).Priority);
        }
    }
}
