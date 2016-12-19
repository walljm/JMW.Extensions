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

namespace JMW.Collections
{
    public class RingBuffer<T> : IEnumerable<T>
    {
        private T[] _array;
        private int _index;
        private int _length;

        public RingBuffer(int size)
        {
            _array = new T[size];
            _index = 0;
            _length = 0;
        }

        public RingBuffer(int size, bool reverse) : this(size)
        {
            ReverseIteration = reverse;
        }

        public RingBuffer(T[] array)
        {
            _array = new T[array.Length];
            Array.Copy(array, _array, array.Length);
            _index = 0;
            _length = array.Length;
        }

        public RingBuffer(T[] array, bool reverse) : this(array)
        {
            ReverseIteration = reverse;
        }

        public RingBuffer(int size, T[] array)
        {
            if (size < array.Length)
                size = array.Length;

            _array = new T[size];
            Array.Copy(array, _array, array.Length);
            _index = 0;
            _length = array.Length;
        }

        public RingBuffer(int size, T[] array, bool reverse) : this(size, array)
        {
            ReverseIteration = reverse;
        }

        public int Count => _length;
        public bool IsReadOnly => false;
        public bool ReverseIteration { get; set; } = false;

        public IEnumerator<T> GetEnumerator()
        {
            if (ReverseIteration)
                return new RingReverseEnumerator<T>(_index, _array);

            return new RingEnumerator<T>(_index, _array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _array[_index] = item;

            _length = _length >= _array.Length ? _array.Length : ++_length;
            _index = _index < _array.Length - 1 ? ++_index : 0;
        }

        public T Last()
        {
            if (ReverseIteration)
                return _array[_index];

            return _index > 0 ? _array[_index - 1] : _array[_length - 1];
        }

        public void Clear()
        {
            for (int i = 0; i < _array.Length; i++)
                _array[i] = default(T);

            _length = 0;
            _index = 0;
        }

        public bool Contains(T item)
        {
            return _array.Contains(item);
        }
    }

    public class RingReverseEnumerator<T> : IEnumerator<T>
    {
        private int _cnt = 0;
        private int _origin;
        private int _idx;
        private T[] _arr;

        public RingReverseEnumerator(int idx, T[] arr)
        {
            _idx = idx; _origin = idx;
            _arr = arr;
        }

        public bool MoveNext()
        {
            _cnt++;
            if (_idx > 0)
                _idx--;
            else
                _idx = _arr.Length - 1;
            return _cnt <= _arr.Length;
        }

        public void Reset()
        {
            _idx = _origin;
        }

        T IEnumerator<T>.Current => _arr[_idx];
        public object Current => _arr[_idx];

        public void Dispose()
        {
        }
    }

    public class RingEnumerator<T> : IEnumerator<T>
    {
        private int _cnt = 0;
        private int _origin;
        private int _idx;
        private T[] _arr;

        public RingEnumerator(int idx, T[] arr)
        {
            _idx = idx - 1; _origin = idx - 1;
            _arr = arr;
        }

        public bool MoveNext()
        {
            _cnt++;
            if (_idx < _arr.Length - 1)
                _idx++;
            else
                _idx = 0;

            return _cnt <= _arr.Length;
        }

        public void Reset()
        {
            _idx = _origin;
        }

        T IEnumerator<T>.Current => _arr[_idx];
        public object Current => _arr[_idx];

        public void Dispose()
        {
        }
    }
}