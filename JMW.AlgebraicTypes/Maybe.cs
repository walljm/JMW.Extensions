using System;

namespace JMW.AlgebraicTypes
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
        public void Use(Action<T> if_success, Action<Exception> if_exception)
        {
            if (_Success) if_success?.Invoke(_value);
            else if_exception?.Invoke(_exception);
        }

        /// <summary>
        /// This function allows you to use the converted value and return it.
        /// </summary>
        /// <param name="if_success">A function that gets the converted value and allows you to return it.</param>
        /// <param name="if_exception">A function that gets the exception and allows you to return a value of type T.</param>
        public T Use(Func<T, T> if_success, Func<Exception, T> if_exception)
        {
            if (_Success) return if_success(_value);
            else return if_exception(_exception);
        }
        
        public bool Equals(T other)
        {
            if (_Success) return _value.Equals(other);
            else return false;
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
            return !(left == right)
        }        
        
        #endregion
    }
}