/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;

namespace JMW.Types.Functional
{
    /// <summary>
    /// The Maybe Class is an algebraic data type that allows you to safely work with nullable values and forces you to handle exceptions.
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    public class Maybe<T> : IEquatable<T>
    {
        private T _value;
        private Exception _exception;

        private bool _Success = false;
        /// <summary>
        /// Indicates if the convertion was successful.
        /// </summary>
        public bool Success
        {
            get
            {
                return _Success;
            }
        }

        /// <summary>
        /// Constructor if the conversion failed.
        /// </summary>
        /// <param name="ex"></param>
        public Maybe(Exception ex)
        {
            _exception = ex; _Success = false;
        }

        /// <summary>
        /// Constructor if the converstion succeeded.
        /// </summary>
        /// <param name="val"></param>
        public Maybe(T val)
        {
            if (val == null)
            {
                _Success = false;
                _exception = new ArgumentException("Value was null.");
            }
            else
            {
                _value = val;
                _Success = true;
            }
        }

        /// <summary>
        /// This function allows you to use the converted value.
        /// </summary>
        /// <param name="if_success">The Action to execute on the successfully converted value.</param>
        /// <param name="if_exception">The Action to execute on the Exception produced by the conversion.</param>
        public void Do(Action<T> if_success, Action<Exception> if_exception)
        {
            if (_Success) if_success?.Invoke(_value);
            else if_exception?.Invoke(_exception);
        }

        /// <summary>
        /// This function allows you to use the converted value and return it.
        /// </summary>
        /// <param name="if_success">A function that gets the converted value and allows you to return it.</param>
        /// <param name="if_exception">A function that gets the exception and allows you to return a value of type T.</param>
        public T Do(Func<T, T> if_success, Func<Exception, T> if_exception)
        {
            if (_Success) return if_success(_value);
            else return if_exception(_exception);
        }

        public bool Equals(T other)
        {
            if (_Success) return _value.Equals(other);

            return false;
        }

        public bool Equals(Maybe<T> other)
        {
            var r = false;

            if (_Success)
            {
                other.Do(if_success: data => r = data.Equals(_value), if_exception: err => { });
            }

            return r;
        }

        #region Operators

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Maybe<T> left, T right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, Maybe<T> right)
        {
            return right == left;
        }

        public static bool operator !=(T left, Maybe<T> right)
        {
            return !(right == left);
        }

        #endregion Operators
    }
}