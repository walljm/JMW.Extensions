using JMW.AlgebraicTypes;
using System;

namespace JMW.Extensions.Conversions
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string into an <typeparamref name="Int16"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Int16> ToInt16(this string s)
        {
            Int16 v;
            if (Int16.TryParse(s, out v))
            {
                return new Maybe<Int16>(v);
            }

            return new Maybe<Int16>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt16"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<UInt16> ToUInt16(this string s)
        {
            UInt16 v;
            if (UInt16.TryParse(s, out v))
            {
                return new Maybe<UInt16>(v);
            }

            return new Maybe<UInt16>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Int32"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Int32> ToInt32(this string s)
        {
            Int32 v;
            if (Int32.TryParse(s, out v))
            {
                return new Maybe<Int32>(v);
            }

            return new Maybe<Int32>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt32"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<UInt32> ToUInt32(this string s)
        {
            UInt32 v;
            if (UInt32.TryParse(s, out v))
            {
                return new Maybe<UInt32>(v);
            }

            return new Maybe<UInt32>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Int64"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Int64> ToInt64(this string s)
        {
            Int64 v;
            if (Int64.TryParse(s, out v))
            {
                return new Maybe<Int64>(v);
            }

            return new Maybe<Int64>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="UInt64"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<UInt64> ToUInt64(this string s)
        {
            UInt64 v;
            if (UInt64.TryParse(s, out v))
            {
                return new Maybe<UInt64>(v);
            }

            return new Maybe<UInt64>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Decimal"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Decimal> ToDecimal(this string s)
        {
            Decimal v;
            if (Decimal.TryParse(s, out v))
            {
                return new Maybe<Decimal>(v);
            }

            return new Maybe<Decimal>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Single"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Single> ToSingle(this string s)
        {
            Single v;
            if (Single.TryParse(s, out v))
            {
                return new Maybe<Single>(v);
            }

            return new Maybe<Single>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="float"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<float> ToFloat(this string s)
        {
            float v;
            if (float.TryParse(s, out v))
            {
                return new Maybe<float>(v);
            }

            return new Maybe<float>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeparamref name="Double"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<Double> ToDouble(this string s)
        {
            Double v;
            if (Double.TryParse(s, out v))
            {
                return new Maybe<Double>(v);
            }

            return new Maybe<Double>(new ArgumentException("Argument " + s + " is not a number."));
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
        public static Maybe<Boolean> ToBoolean(this string s)
        {
            Boolean v;
            if (Boolean.TryParse(s, out v))
            {
                return new Maybe<Boolean>(v);
            }
            var s1 = s.ToLower().Trim();
            if (s1 == "yes" || s1 == "1" || s1 == "on" || s1 == "t" || s1 == "y") return new Maybe<Boolean>(true);
            if (s1 == "no" || s1 == "0" || s1 == "off" || s1 == "f" || s1 == "n") return new Maybe<Boolean>(false);

            return new Maybe<Boolean>(new ArgumentException("Argument " + s + " is not a true/false, yes/no, 0/1, or on/off."));
        }
    }
}