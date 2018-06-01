using System;
using System.Collections;
using System.Collections.Generic;

namespace JMW
{
    /// <summary>
    /// A pair of items of the same type.
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    public class Pair<T> : IStructuralEquatable, IStructuralComparable, IComparable, IEquatable<Pair<T>>, IEqualityComparer<Pair<T>>, IComparable<Pair<T>>
    {
        public Pair(T a, T b)
        {
            A = a;
            B = b;
        }

        public T A { get; }
        public T B { get; }

        #region IComparable

        public static bool operator ==(Pair<T> x, Pair<T> y)
        {
            if (x is null && y is null)
                return true;
            if (x is null) // y can't be null because of the line above.
                return false;
            if (y is null) // x can't be null because of the line above.
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Pair<T> x, Pair<T> y)
        {
            if (x is null && y is null)
                return false;
            if (x is null) // y can't be null because of the line above.
                return true;
            if (y is null) // x can't be null because of the line above.
                return true;

            return !x.Equals(y);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;

            var objTuple = other as Pair<T>;

            if (objTuple == null)
                return false;

            return comparer.Equals(A, objTuple.A) && comparer.Equals(B, objTuple.B);
        }

        public override bool Equals(object obj)
        {
            // use the comparer for the type.
            return Equals((Pair<T>)obj);
        }

        public bool Equals(Pair<T> other)
        {
            if (other == null)
                return false;

            return EqualityComparer<T>.Default.Equals(A, other.A) && EqualityComparer<T>.Default.Equals(B, other.B);
        }

        public bool Equals(Pair<T> x, Pair<T> y)
        {
            return x == y;
        }

        public int CompareTo(Pair<T> other)
        {
            return ((IStructuralComparable)this).CompareTo(other, Comparer<T>.Default);
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<T>.Default);
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
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(A) * 397) ^ EqualityComparer<T>.Default.GetHashCode(B);
            }
        }

        public int GetHashCode(Pair<T> obj)
        {
            return obj.GetHashCode();
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
    public class Pair<Ta, Tb> : IStructuralEquatable, IStructuralComparable, IComparable, IEquatable<Pair<Ta, Tb>>, IEqualityComparer<Pair<Ta, Tb>>, IComparable<Pair<Ta, Tb>>
    {
        public Pair(Ta a, Tb b)
        {
            A = a;
            B = b;
        }

        public Ta A { get; }
        public Tb B { get; }

        #region IComparable

        public static bool operator ==(Pair<Ta, Tb> x, Pair<Ta, Tb> y)
        {
            if (x is null && y is null)
                return true;
            if (x is null) // y can't be null because of the line above.
                return false;
            if (y is null) // x can't be null because of the line above.
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Pair<Ta, Tb> x, Pair<Ta, Tb> y)
        {
            if (x is null && y is null)
                return false;
            if (x is null) // y can't be null because of the line above.
                return true;
            if (y is null) // x can't be null because of the line above.
                return true;

            return !x.Equals(y);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var objTuple = other as Pair<Ta, Tb>;

            if (objTuple == null)
            {
                return false;
            }

            return Equals(objTuple);
        }

        public override bool Equals(object obj)
        {
            return Equals((Pair<Ta, Tb>)obj);
        }

        public bool Equals(Pair<Ta, Tb> other)
        {
            if (other == null)
                return false;

            return EqualityComparer<Ta>.Default.Equals(A, other.A) && EqualityComparer<Tb>.Default.Equals(B, other.B);
        }

        public bool Equals(Pair<Ta, Tb> x, Pair<Ta, Tb> y)
        {
            return x == y;
        }

        public int CompareTo(Pair<Ta, Tb> other)
        {
            return ((IStructuralComparable)this).CompareTo(other, new PairComparer<Ta, Tb>());
        }

        int IComparable.CompareTo(object other)
        {
            return ((IStructuralComparable)this).CompareTo(other, new PairComparer<Ta, Tb>());
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            var objTuple = other as Pair<Ta, Tb>;

            if (objTuple == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return comparer.Compare(this, objTuple);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(comparer.GetHashCode(A), comparer.GetHashCode(A));
        }

        public int GetHashCode(Pair<Ta, Tb> obj)
        {
            return CombineHashCodes(obj.A.GetHashCode(), obj.B.GetHashCode());
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        #endregion IComparable
    }

    public class PairComparer<Ta, Tb> : IComparer<Pair<Ta, Tb>>, IComparer
    {
        public int Compare(Pair<Ta, Tb> x, Pair<Ta, Tb> y)
        {
            // we will ignore the comparer here, because i want to ensure the sub types are compared, not the
            if (x is null && y is null)
                return 0;
            if (x is null) // y can't be null because of the line above.
                return 1;
            if (y is null) // x can't be null because of the line above.
                return -1;

            var c = 0;

            c = Comparer<Ta>.Default.Compare(x.A, y.A);

            if (c != 0) return c;

            return Comparer<Tb>.Default.Compare(x.B, y.B);
        }

        public int Compare(object x, object y)
        {
            if (x is null && y is null)
                return 0;
            if (x is null) // y can't be null because of the line above.
                return 1;
            if (y is null) // x can't be null because of the line above.
                return -1;

            var xT = x as Pair<Ta, Tb>;
            var yT = y as Pair<Ta, Tb>;

            return Compare(xT, yT);
        }
    }
}