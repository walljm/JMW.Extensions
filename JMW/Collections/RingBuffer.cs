/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Collections;

public class RingBuffer<T> : IEnumerable<T>
{
    private readonly T[] internalArray;
    private int index;

    public RingBuffer(int size)
    {
        internalArray = new T[size];
        index = 0;
        Count = 0;
    }

    public RingBuffer(int size, bool reverse) : this(size)
    {
        ReverseIteration = reverse;
    }

    public RingBuffer(T[] array)
    {
        internalArray = new T[array.Length];
        Array.Copy(array, internalArray, array.Length);
        index = 0;
        Count = array.Length;
    }

    public RingBuffer(T[] array, bool reverse) : this(array)
    {
        ReverseIteration = reverse;
    }

    public RingBuffer(int size, T[] array)
    {
        if (size < array.Length)
            size = array.Length;

        internalArray = new T[size];
        Array.Copy(array, internalArray, array.Length);
        index = 0;
        Count = array.Length;
    }

    public RingBuffer(int size, T[] array, bool reverse) : this(size, array)
    {
        ReverseIteration = reverse;
    }

    public int Count { get; private set; }

    public bool IsReadOnly => false;
    public bool ReverseIteration { get; set; } = false;

    public IEnumerator<T> GetEnumerator()
    {
        if (ReverseIteration)
            return new RingReverseEnumerator<T>(index, internalArray, Count);

        return new RingEnumerator<T>(index, internalArray, Count);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        internalArray[index] = item;

        Count = Count >= internalArray.Length ? internalArray.Length : ++Count;
        index = index < internalArray.Length - 1 ? ++index : 0;
    }

    public T Last()
    {
        if (ReverseIteration)
            return internalArray[index];

        return index > 0 ? internalArray[index - 1] : internalArray[Count - 1];
    }

    public T First()
    {
        return internalArray[index];
    }

    public void Clear()
    {
        for (var i = 0; i < internalArray.Length; i++)
            internalArray[i] = default;

        Count = 0;
        index = 0;
    }

    public bool Contains(T item)
    {
        return internalArray.Contains(item);
    }

    public T[] ToArray()
    {
        var arr = new T[Count];

        var j = 0;
        foreach (var i in this)
        {
            arr[j++] = i;
        }

        return arr;
    }

    public List<T> ToList()
    {
        return new List<T>(this);
    }

    public T this[int index]
    {
        get
        {
            if (index >= Count)
                throw new IndexOutOfRangeException("Index: " + index + " is not less than " + Count);
            var idx = this.index + index < this.internalArray.Length ? this.index + index : this.index + index - this.internalArray.Length;
            return internalArray[idx];
        }

        set
        {
            if (index >= Count)
                throw new IndexOutOfRangeException("Index: " + index + " is not less than " + Count);

            var idx = this.index + index < this.internalArray.Length ? this.index + index : this.index + index - this.internalArray.Length;
            internalArray[idx] = value;
        }
    }
}

public class RingReverseEnumerator<T> : IEnumerator<T>
{
    private int count = 0;
    private int index;
    private readonly int origin;
    private readonly int length;
    private readonly T[] internalArray;

    public RingReverseEnumerator(int idx, T[] arr, int length)
    {
        index = idx; origin = idx;
        internalArray = arr;
        this.length = length;
    }

    public bool MoveNext()
    {
        count++;
        if (index > 0)
            index--;
        else
            index = length - 1;
        return count <= length;
    }

    public void Reset()
    {
        index = origin;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    T IEnumerator<T>.Current => internalArray[index];
    public object Current => internalArray[index];
}

public class RingEnumerator<T> : IEnumerator<T>
{
    private int count = 0;
    private int index;
    private readonly int origin;
    private readonly int length;
    private readonly T[] internalArray;

    public RingEnumerator(int idx, T[] arr, int length)
    {
        index = idx - 1; origin = idx - 1;
        internalArray = arr;
        this.length = length;
    }

    public bool MoveNext()
    {
        count++;
        if (index < length - 1)
            index++;
        else
            index = 0;

        return count <= length;
    }

    public void Reset()
    {
        index = origin;
    }

    T IEnumerator<T>.Current => internalArray[index];
    public object Current => internalArray[index];

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}