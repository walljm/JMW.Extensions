using System;
using System.Collections;
using System.Collections.Generic;

namespace JMW
{
    /// <summary>
    /// A pair of items of the same type.
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    public class Pair<T> : IStructuralEquatable, IStructuralComparable, IComparable
    {
        public Pair(T a, T b)
        {
            A = a;
            B = b;
        }

        public T A { get; }
        public T B { get; }

        #region IComparable

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var objTuple = other as Pair<T>;

            if (objTuple == null)
            {
                return false;
            }

            return comparer.Equals(A, objTuple.A) && comparer.Equals(B, objTuple.B);
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            var objTuple = other as Pair<T>;

            if (objTuple == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var c = 0;

            c = comparer.Compare(A, objTuple.A);

            if (c != 0) return c;

            return comparer.Compare(B, objTuple.B);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(comparer.GetHashCode(A), comparer.GetHashCode(A));
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        #endregion IComparable
    }

    /// <summary>
    /// A pair of items of different types.
    /// </summary>
    /// <typeparam name="Ta">The first type of object being compared</typeparam>
    /// <typeparam name="Tb">The second type of object being compared.</typeparam>
    public class Pair<Ta, Tb> : IStructuralEquatable, IStructuralComparable, IComparable
    {
        public Pair(Ta a, Tb b)
        {
            A = a;
            B = b;
        }

        public Ta A { get; }
        public Tb B { get; }

        #region IComparable

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var objTuple = other as Pair<Ta, Tb>;

            if (objTuple == null)
            {
                return false;
            }

            return comparer.Equals(A, objTuple.A) && comparer.Equals(B, objTuple.B);
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            var objTuple = other as Pair<Ta, Tb>;

            if (objTuple == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var c = 0;

            c = comparer.Compare(A, objTuple.A);

            if (c != 0) return c;

            return comparer.Compare(B, objTuple.B);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(comparer.GetHashCode(A), comparer.GetHashCode(A));
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        #endregion IComparable
    }
}