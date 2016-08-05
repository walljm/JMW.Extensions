using System;
using System.Collections.Generic;
using JMW.Extensions.Object;

namespace JMW.Differentials
{
    public static class Differerential
    {
        /// <summary>
        ///   Takes two indexed collections and performs a comparison.
        /// </summary>
        /// <typeparam name="T">The type of object being compared</typeparam>
        /// <typeparam name="M">The type of the object storing the difference.</typeparam>
        /// <param name="lstA">List A</param>
        /// <param name="lstB">List B</param>
        /// <param name="retriever">A function that takes an object and list of objects and returns an object from the list that matches the supplied object or returns null if no matching object exists in the supplied list.</param>
        /// <param name="comparer">A function that compares two objects found in both lists and returns an <seealso cref="ObjectDifferential{T, M}"/></param>
        /// <returns></returns>
        public static CollectionDifferential<T, M> DiffList<T, M>(
            IList<T> lstA,
            IList<T> lstB,
            Func<T, IList<T>, T> retriever,
            Func<T, T, ObjectDifferential<T, M>> comparer)
        {
            // some objects to hold things.
            var diff = new CollectionDifferential<T, M>();
            var same = new List<Tuple<T, T>>();

            // loop through all the objects in lstA and see if there is an equivalent object in lstB
            for (int i = 0; i < lstA.Count; i++)
            {
                var a_item = lstA[i];
                var b_item = retriever(a_item, lstB);
                if (b_item != null)
                {
                    // you found an object, so store it and its equivalent form lstB in the "same" collection
                    same.Add(new Tuple<T, T>(a_item, b_item));
                }
                else // no object found, it exists only in lstA
                    diff.OnlyInListA.Add(a_item);
            }

            // loop through all the objects in lstB and see if there is an equivalent in lstA
            for (int i = 0; i < lstB.Count; i++)
            {
                var b_item = lstB[i];
                var a_item = retriever(b_item, lstA);

                // you only store the ones that fail because you've already stored the items that are the same
                //   when you iterated through lstA
                if (a_item == null)
                    diff.OnlyInListB.Add(b_item);
            }

            // loop an figure the differences between each object that exists in both.
            foreach (var t in same)
            {
                // for the items that are both in the list, compare to see if they are different.
                var item = comparer(t.Item1, t.Item2);
                if (item.IsNotNull())
                    diff.Modified.Add(item);
            }

            return diff;
        }
    }

    /// <summary>
    ///  A Class that holds the differences between two lists of objects.
    /// </summary>
    /// <typeparam name="T">The type of object being compared</typeparam>
    /// <typeparam name="M">The type of the object storing the difference.</typeparam>
    public class CollectionDifferential<T, M>
    {
        /// <summary>
        /// The objects that exist only in List A
        /// </summary>
        public List<T> OnlyInListA { get; } = new List<T>();

        /// <summary>
        /// The objects that exist only in List B
        /// </summary>
        public List<T> OnlyInListB { get; } = new List<T>();

        /// <summary>
        /// The objects that exist in both List A and List B, and how they are different.
        /// </summary>
        public List<ObjectDifferential<T, M>> Modified { get; } = new List<ObjectDifferential<T, M>>();
    }

    /// <summary>
    /// Holds the differences between two objects.
    /// </summary>
    /// <typeparam name="T">The type of object being compared</typeparam>
    /// <typeparam name="M">The type of the object storing the difference.</typeparam>
    public class ObjectDifferential<T, M>
    {
        public ObjectDifferential(T objA, T objB, List<M> diffs)
        {
            ObjectA = objA;
            ObjectB = objB;
            Differences = diffs;
        }

        public T ObjectA { get; private set; }
        public T ObjectB { get; private set; }
        public List<M> Differences { get; private set; }
    }
}