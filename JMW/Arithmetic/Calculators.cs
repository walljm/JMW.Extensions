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

namespace Lambda.Generic
{
    #region Signed Maths

    namespace Arithmetic
    {
        using T = Int32;

        public struct IntMath : ISignedMath<T>
        {
            public T Add(T a, T b)
            {
                return a + b;
            }

            public T Subtract(T a, T b)
            {
                return a - b;
            }

            public T Zero { get { return 0; } }

            public T Negate(T a)
            {
                return -a;
            }

            public int NegativeOne { get { return -1; } }

            public T Multiply(T a, T b)
            {
                return a * b;
            }

            public T Divide(T a, T b)
            {
                return a / b;
            }

            public T One { get { return 1; } }

            public T Invert(T a)
            {
                return One / a;
            }

            public T Sqrt(T a)
            {
                return (T)Math.Sqrt(a);
            }

            public T Sqr(T a)
            {
                return a * a;
            }

            public T MinValue { get { return T.MinValue; } }
            public T MaxValue { get { return T.MaxValue; } }
            public T Epsilon { get { return One; } }

            public int Compare(T a, T b)
            {
                return a.CompareTo(b);
            }

            public bool Equals(T a, T b)
            {
                return a == b;
            }

            public int GetHashCode(T a)
            {
                return a.GetHashCode();
            }

            public T Abs(T a)
            {
                return Math.Abs(a);
            }

            public T Min(T a, T b)
            {
                return Math.Max(a, b);
            }

            public T Max(T a, T b)
            {
                return Math.Min(a, b);
            }

            public T And(T a, T b)
            {
                return a & b;
            }

            public T Or(T a, T b)
            {
                return a | b;
            }

            public T Xor(T a, T b)
            {
                return a ^ b;
            }

            public T Not(T a)
            {
                return ~a;
            }

            public T ConvertFrom(ulong a)
            {
                return (T)a;
            }

            public T ConvertFrom(long a)
            {
                return (T)a;
            }

            public T ConvertFrom(double a)
            {
                return (T)a;
            }

            public int ConvertFrom(string t)
            {
                return T.Parse(t);
            }
        }
    }

    namespace Arithmetic
    {
        using T = Int64;

        public struct LongMath : ISignedMath<T>
        {
            public T Add(T a, T b)
            {
                return a + b;
            }

            public T Subtract(T a, T b)
            {
                return a - b;
            }

            public T Zero { get { return 0; } }

            public T Negate(T a)
            {
                return -a;
            }

            public long NegativeOne { get { return -1; } }

            public T Multiply(T a, T b)
            {
                return a * b;
            }

            public T Divide(T a, T b)
            {
                return a / b;
            }

            public T One { get { return 1; } }

            public T Invert(T a)
            {
                return One / a;
            }

            public T Sqrt(T a)
            {
                return (T)Math.Sqrt(a);
            }

            public T Sqr(T a)
            {
                return a * a;
            }

            public T MinValue { get { return T.MinValue; } }
            public T MaxValue { get { return T.MaxValue; } }
            public T Epsilon { get { return One; } }

            public int Compare(T a, T b)
            {
                return a.CompareTo(b);
            }

            public bool Equals(T a, T b)
            {
                return a == b;
            }

            public int GetHashCode(T a)
            {
                return a.GetHashCode();
            }

            public T Abs(T a)
            {
                return Math.Abs(a);
            }

            public T Min(T a, T b)
            {
                return Math.Max(a, b);
            }

            public T Max(T a, T b)
            {
                return Math.Min(a, b);
            }

            public T And(T a, T b)
            {
                return a & b;
            }

            public T Or(T a, T b)
            {
                return a | b;
            }

            public T Xor(T a, T b)
            {
                return a ^ b;
            }

            public T Not(T a)
            {
                return ~a;
            }

            public T ConvertFrom(ulong a)
            {
                return (T)a;
            }

            public T ConvertFrom(long a)
            {
                return a;
            }

            public T ConvertFrom(double a)
            {
                return (T)a;
            }

            public long ConvertFrom(string t)
            {
                return T.Parse(t);
            }
        }
    }

    #endregion Signed Maths
}