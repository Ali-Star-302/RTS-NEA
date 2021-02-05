using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<Node> where Node : IHeapElement<Node>
{
    Node[] items;
    int itemCount;

    public Heap(int maxHeapSize)
    {
        items = new Node[maxHeapSize];
    }

    public void Add(Node item)
    {
        item.GetHeapIndex = itemCount;
        items[itemCount] = item;
        SortUp(item);
        itemCount++;
    }

    public Node RemoveFirst()
    {
        Node firstItem = items[0];
        itemCount--;
        items[0] = items[itemCount];
        items[0].GetHeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(Node item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return itemCount;
        }
    }

    public bool Contains(Node item)
    {
        return Equals(items[item.GetHeapIndex], item);
    }

    void SortDown(Node item)
    {
        while (true)
        {
            int childIndexLeft = item.GetHeapIndex * 2 + 1;
            int childIndexRight = item.GetHeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < itemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < itemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(Node item)
    {
        int parentIndex = (item.GetHeapIndex - 1) / 2;
        while (true)
        {
            Node parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    void Swap(Node itemA, Node itemB)
    {
        items[itemA.GetHeapIndex] = itemB;
        items[itemB.GetHeapIndex] = itemA;
        int itemAIndex = itemA.GetHeapIndex;
        itemA.GetHeapIndex = itemB.GetHeapIndex;
        itemB.GetHeapIndex = itemAIndex;
    }
}

public interface IHeapElement<T> : IComparable<T>
{
    int GetHeapIndex
    {
        get;
        set;
    }
}