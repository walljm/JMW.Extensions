/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using JMW.Functional;
using System;

namespace JMW.Extensions.Conversions
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string into an <typeref name="short"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<short> ToInt16(this string s)
        {
            short v;
            if (short.TryParse(s, out v))
            {
                return new Maybe<short>(v);
            }

            return new Maybe<short>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="ushort"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<ushort> ToUInt16(this string s)
        {
            ushort v;
            if (ushort.TryParse(s, out v))
            {
                return new Maybe<ushort>(v);
            }

            return new Maybe<ushort>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="int"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<int> ToInt32(this string s)
        {
            int v;
            if (int.TryParse(s, out v))
            {
                return new Maybe<int>(v);
            }

            return new Maybe<int>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="uint"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<uint> ToUInt32(this string s)
        {
            uint v;
            if (uint.TryParse(s, out v))
            {
                return new Maybe<uint>(v);
            }

            return new Maybe<uint>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="long"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<long> ToInt64(this string s)
        {
            long v;
            if (long.TryParse(s, out v))
            {
                return new Maybe<long>(v);
            }

            return new Maybe<long>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="ulong"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<ulong> ToUInt64(this string s)
        {
            ulong v;
            if (ulong.TryParse(s, out v))
            {
                return new Maybe<ulong>(v);
            }

            return new Maybe<ulong>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="decimal"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<decimal> ToDecimal(this string s)
        {
            decimal v;
            if (decimal.TryParse(s, out v))
            {
                return new Maybe<decimal>(v);
            }

            return new Maybe<decimal>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into an <typeref name="float"/> safely.
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
        /// Converts a string into an <typeref name="double"/> safely.
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<double> ToDouble(this string s)
        {
            double v;
            if (double.TryParse(s, out v))
            {
                return new Maybe<double>(v);
            }

            return new Maybe<double>(new ArgumentException("Argument " + s + " is not a number."));
        }

        /// <summary>
        /// Converts a string into a <typeref name="bool"/> safely, and intuitively.
        ///
        ///  true/false
        ///  0/1
        ///  yes/no
        ///  on/off
        ///  y/n
        ///  t/f
        /// </summary>
        /// <param name="s">String to convert.</param>
        public static Maybe<bool> ToBoolean(this string s)
        {
            bool v;
            if (bool.TryParse(s, out v))
            {
                return new Maybe<bool>(v);
            }
            var s1 = s.ToLower().Trim();
            if (s1 == "yes" || s1 == "1" || s1 == "on" || s1 == "t" || s1 == "y") return new Maybe<bool>(true);
            if (s1 == "no" || s1 == "0" || s1 == "off" || s1 == "f" || s1 == "n") return new Maybe<bool>(false);

            return new Maybe<bool>(new ArgumentException("Argument " + s + " is not a true/false, yes/no, 0/1, or on/off."));
        }
    }
}