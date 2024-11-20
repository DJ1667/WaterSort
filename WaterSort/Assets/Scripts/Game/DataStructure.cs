using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 堆

/// <summary>
/// 小顶堆
/// </summary>
public class MinHeap<T> where T : IComparable<T>
{
    public int Count => heap.Count;

    private List<T> heap;

    public MinHeap()
    {
        heap = new List<T>();
    }

    /// <summary>
    /// 插入元素
    /// </summary>
    /// <param name="value"></param>
    public void Insert(T value)
    {
        heap.Add(value);
        HeapifyUp(heap.Count - 1);
    }

    /// <summary>
    /// 删除最小元素并返回
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (heap.Count == 0)
        {
            Debug.LogError("Heap is empty.");
            return default(T);
        }

        T min = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return min;
    }

    public void Remove(T value)
    {
        int index = heap.IndexOf(value);
        if (index == -1)
        {
            Debug.LogError("Value not found.");
            return;
        }

        heap[index] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(index);
    }

    public bool Contains(T value)
    {
        return heap.Contains(value);
    }

    /// <summary>
    /// 获取最小元素
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        if (heap.Count == 0)
        {
            Debug.LogError("Heap is empty.");
            return default(T);
        }

        return heap[0];
    }

    // 堆化向上
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].CompareTo(heap[parentIndex]) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    // 堆化向下
    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;
        while (index < lastIndex)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= lastIndex && heap[leftChildIndex].CompareTo(heap[smallestIndex]) < 0)
            {
                smallestIndex = leftChildIndex;
            }
            if (rightChildIndex <= lastIndex && heap[rightChildIndex].CompareTo(heap[smallestIndex]) < 0)
            {
                smallestIndex = rightChildIndex;
            }
            if (smallestIndex == index)
                break;

            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }

    // 交换元素
    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }

    // 获取当前堆的大小
    public int Size()
    {
        return heap.Count;
    }
}

/// <summary>
/// 大顶堆
/// </summary>
/// <typeparam name="T"></typeparam>
public class MaxHeap<T> where T : IComparable<T>
{
    public int Count => heap.Count;

    private List<T> heap;

    public MaxHeap()
    {
        heap = new List<T>();
    }

    /// <summary>
    /// 插入元素
    /// </summary>
    /// <param name="value"></param>
    public void Insert(T value)
    {
        heap.Add(value);
        HeapifyUp(heap.Count - 1);
    }

    /// <summary>
    /// 删除最大元素并返回
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (heap.Count == 0)
        {
            Debug.LogError("Heap is empty.");
            return default(T);
        }

        T max = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return max;
    }

    /// <summary>
    /// 获取最大元素
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        if (heap.Count == 0)
        {
            Debug.LogError("Heap is empty.");
            return default(T);
        }

        return heap[0];
    }

    // 堆化向上
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].CompareTo(heap[parentIndex]) > 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    // 堆化向下
    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;
        while (index < lastIndex)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int largestIndex = index;

            if (leftChildIndex <= lastIndex && heap[leftChildIndex].CompareTo(heap[largestIndex]) > 0)
            {
                largestIndex = leftChildIndex;
            }
            if (rightChildIndex <= lastIndex && heap[rightChildIndex].CompareTo(heap[largestIndex]) > 0)
            {
                largestIndex = rightChildIndex;
            }
            if (largestIndex == index)
                break;

            Swap(index, largestIndex);
            index = largestIndex;
        }
    }

    // 交换元素
    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }

    // 获取当前堆的大小
    public int Size()
    {
        return heap.Count;
    }
}

#endregion