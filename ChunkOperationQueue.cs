﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

public enum ChunkOperation{
    Detail,UnDetail,UnLoad,None
}
public enum ChunkState{
    Detailed,UnDetailed,UnLoaded,None
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChunkOperationQueue : UdonSharpBehaviour
{
    private Vector4[] _array;
    private int _head; // First valid element in the queue.
    private int _tail; // Last valid element in the queue.
    private int _size; // Number of elements.
    private int _version;

    private const int MinimumGrow = 100000;

    public int Count => _size;
    public int Version => _version;

    [Obsolete("Use Count Property.")]
    public int GetCount() => _size;
    [Obsolete("Use Version Property.")]
    public int GetVersion() => _version;

    

    // Removes all Objects from the queue.
    public void Clear()
    {
        if (_size != 0)
        {
            if (_head < _tail)
            {
                Array.Clear(_array, _head, _size);
            }
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _size = 0;
        }

        _head = 0;
        _tail = 0;
        _version++;
    }

    // CopyTo copies a collection into an Array, starting at a particular
    // index into the array.

    // Adds obj to the tail of the queue.
    public void Enqueue(Vector4 obj)
    {
        if (_array == null)
        {
            SetCapacity(MinimumGrow);
        }
        else if (_size == _array.Length)
        {
            SetCapacity(_array.Length + MinimumGrow);
        }

        _array[_tail] = obj;
        _tail = (_tail + 1) % _array.Length;
        _size++;
        _version++;
    }

    // Removes the int at the head of the queue and returns it. If the queue
    // is empty, this method returns null.
    public Vector4 Dequeue()
    {
        if (_size == 0)
            Debug.LogError("Queue is empty!");

        Vector4 removed = _array[_head];
        _array[_head] = Vector4.zero;
        _head = (_head + 1) % _array.Length;
        _size--;
        _version++;
        return removed;
    }

    // Returns the int at the head of the queue. The int remains in the
    // queue. If the queue is empty, this method returns null.
    public Vector4 Peek()
    {
        if (_size == 0)
        {
            //throw new InvalidOperationException();
            return Vector4.zero;
        }

        return _array[_head];
    }

    // Iterates over the ints in the queue, returning an array of the
    // ints in the Queue, or an empty array if the queue is empty.
    // The order of elements in the array is first in to last in, the same
    // order produced by successive calls to Dequeue.

    // PRIVATE Grows or shrinks the buffer to hold capacity ints. Capacity
    // must be >= _size.
    public void SetCapacity(int capacity)
    {
        Vector4[] newArray = new Vector4[capacity];
        if (_size == 0)
        {

        }
        else if (_head < _tail)
        {
            Debug.LogWarning("ChunkOperationQueue: SetCapacity: _head < _tail");
            Array.Copy(_array, _head, newArray, 0, _size);
        }
        else
        {
            Debug.LogWarning("ChunkOperationQueue: SetCapacity: _head >= _tail");
            int toHead = _array.Length - _head;
            Array.Copy(_array, _head, newArray, 0, toHead);
            Array.Copy(_array, 0, newArray, toHead, _tail);
        }

        _array = newArray;
        _head = 0;
        _tail = _size == capacity ? 0 : _size;
        _version++;
    }
}