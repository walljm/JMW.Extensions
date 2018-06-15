// Copyright (c) 2004, Rüdiger Klaehn
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of lambda computing nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;

namespace Lambda.Generic.Arithmetic
{
    #region Signed Wrapper

    public struct Signed<T, O> : IComparable<T>, IComparable, IFormattable, IConvertible, IEquatable<T>
        where O : ISignedMath<T>, new()
        where T : IComparable<T>, IComparable, IFormattable, IConvertible, IEquatable<T>
    {
        private static readonly O o = new O();
        internal readonly T value;

        public Signed(T a)
        {
            value = a;
        }

        public static explicit operator Signed<T, O>(ulong a)
        {
            return o.ConvertFrom(a);
        }

        public static explicit operator Signed<T, O>(long a)
        {
            return o.ConvertFrom(a);
        }

        public static implicit operator Signed<T, O>(T a)
        {
            return new Signed<T, O>(a);
        }

        public static implicit operator T(Signed<T, O> a)
        {
            return a.value;
        }

        public static Signed<T, O> operator +(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Add(a, b);
        }

        public static Signed<T, O> operator -(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Subtract(a, b);
        }

        public static Signed<T, O> operator -(Signed<T, O> a)
        {
            return o.Negate(a);
        }

        public static Signed<T, O> operator *(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Multiply(a, b);
        }

        public static Signed<T, O> operator /(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Divide(a, b);
        }

        public static Signed<T, O> Zero
        {
            get { return o.Zero; }
        }
        public static Signed<T, O> One
        {
            get { return o.One; }
        }

        public static Signed<T, O> Parse(string s)
        {
            return o.ConvertFrom(s);
        }

        public static bool operator ==(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Equals(a, b);
        }

        public static bool operator !=(Signed<T, O> a, Signed<T, O> b)
        {
            return !o.Equals(a, b);
        }

        public static bool operator <=(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Compare(a, b) <= 0;
        }

        public static bool operator >=(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Compare(a, b) >= 0;
        }

        public static bool operator <(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Compare(a, b) < 0;
        }

        public static bool operator >(Signed<T, O> a, Signed<T, O> b)
        {
            return o.Compare(a, b) > 0;
        }

        public int CompareTo(T other)
        {
            return value.CompareTo(other);
        }

        public bool Equals(T other)
        {
            return value.Equals(other);
        }

        public override bool Equals(object a)
        {
            switch (a)
            {
                case Signed<T, O> _:
                    return value.Equals(((Signed<T, O>)a).value);

                case T _:
                    return value.Equals((T)a);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return o.GetHashCode(value);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return value.ToString(format, formatProvider);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Signed<T, O> _:
                    return value.CompareTo(((Signed<T, O>)obj).value);

                case T _:
                    return value.CompareTo((T)obj);
            }

            return -1;
        }

        #region Implementation of IConvertible

        public TypeCode GetTypeCode()
        {
            return value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return value.ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return value.ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return value.ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return value.ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return value.ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return value.ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return value.ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return value.ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return value.ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return value.ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return value.ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return value.ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return value.ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return value.ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return value.ToType(conversionType, provider);
        }

        #endregion Implementation of IConvertible
    }

    #endregion Signed Wrapper
}