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
        /// Indicates if the conversion was successful.
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
        /// Constructor if the conversion succeeded.
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
        /// This function allows you to use the converted value.  Functions are not executed if they are null.
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
            if (_Success)
            {
                if (if_success == null) throw new ArgumentException("if_success parameter cannot be null.");
                return if_success(_value);
            }
            else
            {
                if (if_exception == null) throw new ArgumentException("if_exception parameter cannot be null.");
                return if_exception(_exception);
            }
        }

        /// <summary>
        /// Does an equality comparison between the provided value of type <typeparamref name="T"/> and the internal value of the <see cref="Maybe{T}"/>.
        /// 
        /// This function returns false if the <see cref="Maybe{T}"/> is not successful.
        /// </summary>
        /// <param name="other">Value to compare.</param>
        /// <returns>True if the <see cref="Maybe{T}"/> was successful and the value is equal to <paramref name="other"/></returns>
        public bool Equals(T other)
        {
            if (_Success) return _value.Equals(other);

            return false;
        }

        /// <summary>
        /// Compares two <see cref="Maybe{T}"/> objects.  
        /// </summary>
        /// <param name="other">Value to compare</param>
        /// <returns>Returns true if both <see cref="Maybe{T}"/> were successful and their values are equal.</returns>
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

        /// <summary>
        ///   Operator overload.  Compares two <see cref="Maybe{T}"/> objects.  
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns true if both <see cref="Maybe{T}"/> were successful and their values are equal, or if both are null.</returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Operator overload. Compares two <see cref="Maybe{T}"/> objects.  
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns false if both <see cref="Maybe{T}"/> were successful and their values are equal, or if both are null.</returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Operator overload.  Compares a <see cref="Maybe{T}"/> object and an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns true if the <see cref="Maybe{T}"/> was successful and their values are equal, or if both are null.</returns>
        public static bool operator ==(Maybe<T> left, T right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operator overload. Compares a <see cref="Maybe{T}"/> object and an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns false if the <see cref="Maybe{T}"/> was successful and their values are equal, or if both are null.</returns>
        public static bool operator !=(Maybe<T> left, T right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Operator overload.  Compares a <see cref="Maybe{T}"/> object and an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns true if the <see cref="Maybe{T}"/> was successful and their values are equal, or if both are null.</returns>
        public static bool operator ==(T left, Maybe<T> right)
        {
            return right == left;
        }

        /// <summary>
        /// Operator overload. Compares a <see cref="Maybe{T}"/> object and an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="left">Left side parameter</param>
        /// <param name="right">Right side parameter</param>
        /// <returns>Returns false if the <see cref="Maybe{T}"/> was successful and their values are equal, or if both are null.</returns>
        public static bool operator !=(T left, Maybe<T> right)
        {
            return !(right == left);
        }

        #endregion Operators
    }
}