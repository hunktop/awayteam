using System.Collections.Generic;
using System;

public class PriorityQueue<Key>
{
    private List<KeyValuePair<Key, int>> heap;
    private Dictionary<Key, int> indexTable;

    public int Count
    {
        get
        {
            return this.heap.Count - 1;
        }
    }

    public PriorityQueue()
        : this(10)
    {
    }

    public PriorityQueue(int capacity)
    {
        this.heap = new List<KeyValuePair<Key, int>>(capacity);
        this.heap.Add(new KeyValuePair<Key, int>());
        this.indexTable = new Dictionary<Key, int>(capacity);
    }

    public void DecreaseKey(Key p, int dist)
    {
        int index;
        if (this.indexTable.TryGetValue(p, out index))
        {
            this.heap[index] = new KeyValuePair<Key, int>(this.heap[index].Key, dist);
            var parent = index >> 1;
            while (index > 1 && this.heap[parent].Value > this.heap[index].Value)
            {
                this.indexTable[this.heap[index].Key] = parent;
                this.indexTable[this.heap[parent].Key] = index;
                var temp = this.heap[index];
                this.heap[index] = this.heap[parent];
                this.heap[parent] = temp;
                index = parent;
                parent = index >> 1;
            }
        }
    }

    public void Insert(Key p, int dist)
    {
        int index;
        if (!this.indexTable.TryGetValue(p, out index))
        {
            this.heap.Add(new KeyValuePair<Key, int>(p, int.MaxValue));
            this.indexTable.Add(p, this.heap.Count - 1);
            this.DecreaseKey(p, dist);
        }
    }

    public KeyValuePair<Key, int> ExtractMin()
    {
        if(this.Count == 0)
        {
            throw new InvalidOperationException("Attempting to extract min from empty priority queue.");
        }

        var min = this.heap[1];
        this.heap[1] = this.heap[this.heap.Count - 1];
        this.heap.RemoveAt(this.heap.Count - 1);
        this.indexTable.Remove(min.Key);

        if (this.Count > 0)
        {
            this.heapify(1);
        }
        return min;
    }

    public bool Contains(Key p)
    {
        return this.indexTable.ContainsKey(p);
    }

    private void heapify(int index)
    {
        var left = (index << 1);
        var right = (index << 1) + 1;
        var minIndex = index;

        if (left < this.heap.Count && this.heap[left].Value < this.heap[minIndex].Value)
        {
            minIndex = left;
        }
        if (right < this.heap.Count && this.heap[right].Value < this.heap[minIndex].Value)
        {
            minIndex = right;
        }

        if (index != minIndex)
        {
            var temp = this.heap[index];
            this.heap[index] = this.heap[minIndex];
            this.heap[minIndex] = temp;
            heapify(minIndex);
        }
    }

    public bool ContainsKey(Key adj)
    {
        return this.indexTable.ContainsKey(adj);
    }
}
