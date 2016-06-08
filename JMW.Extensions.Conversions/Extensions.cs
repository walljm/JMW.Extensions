using System;

namespace JMW.Extensions.Conversions
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string into an <typeparamref name="Int16"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Int16> ToInt16(this string s)
        {
            Int16 v;
            if (Int16.TryParse(s, out v))
            {
                return new Converted<Int16>(v);
            }

            return new Converted<Int16>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt16"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<UInt16> ToUInt16(this string s)
        {
            UInt16 v;
            if (UInt16.TryParse(s, out v))
            {
                return new Converted<UInt16>(v);
            }

            return new Converted<UInt16>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Int32"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Int32> ToInt32(this string s)
        {
            Int32 v;
            if (Int32.TryParse(s, out v))
            {
                return new Converted<Int32>(v);
            }

            return new Converted<Int32>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt32"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<UInt32> ToUInt32(this string s)
        {
            UInt32 v;
            if (UInt32.TryParse(s, out v))
            {
                return new Converted<UInt32>(v);
            }

            return new Converted<UInt32>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Int64"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Int64> ToInt64(this string s)
        {
            Int64 v;
            if (Int64.TryParse(s, out v))
            {
                return new Converted<Int64>(v);
            }

            return new Converted<Int64>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt64"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<UInt64> ToUInt64(this string s)
        {
            UInt64 v;
            if (UInt64.TryParse(s, out v))
            {
                return new Converted<UInt64>(v);
            }

            return new Converted<UInt64>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Decimal"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Decimal> ToDecimal(this string s)
        {
            Decimal v;
            if (Decimal.TryParse(s, out v))
            {
                return new Converted<Decimal>(v);
            }

            return new Converted<Decimal>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Single"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Single> ToSingle(this string s)
        {
            Single v;
            if (Single.TryParse(s, out v))
            {
                return new Converted<Single>(v);
            }

            return new Converted<Single>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="float"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<float> ToFloat(this string s)
        {
            float v;
            if (float.TryParse(s, out v))
            {
                return new Converted<float>(v);
            }

            return new Converted<float>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Double"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Double> ToDouble(this string s)
        {
            Double v;
            if (Double.TryParse(s, out v))
            {
                return new Converted<Double>(v);
            }

            return new Converted<Double>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into a <typeparamref name="Boolean"/> safely, and intuitively.
        ///
        ///  true/false
        ///  0/1
        ///  yes/no
        ///  on/off
        ///  y/n
        ///  t/f
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Converted<Boolean> ToBoolean(this string s)
        {
            Boolean v;
            if (Boolean.TryParse(s, out v))
            {
                return new Converted<Boolean>(v);
            }
            var s1 = s.ToLower().Trim();
            if (s1 == "yes" || s1 == "1" || s1 == "on" || s1 == "t" || s1 == "y") return new Converted<Boolean>(true);
            if (s1 == "no" || s1 == "0" || s1 == "off" || s1 == "f" || s1 == "n") return new Converted<Boolean>(false);

            return new Converted<Boolean>(new ArgumentException("Argument " + s + " is not a true/false, yes/no, 0/1, or on/off."));
        }
    }

    public class Converted<T>
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
        public Converted(Exception ex)
        {
            _exception = ex; _Success = false;
        }

        /// <summary>
        /// Constructor if the converstion succeeded.
        /// </summary>
        /// <param name="val"></param>
        public Converted(T val)
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
    }
}