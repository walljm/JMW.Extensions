using System.Collections.Generic;
using System.Linq;

namespace JMW.Extensions.Enumerable
{
    public static class Extensions
    {
        /// <summary>
        ///   Return the last items in the list in the specified quantity
        /// </summary>
        /// <typeparam name="T">Type of items in the list</typeparam>
        /// <param name="lst">The list to return items from</param>
        /// <param name="qty">The number of items to return</param>
        public static IEnumerable<T> Last<T>(this IEnumerable<T> lst, int qty = 1)
        {
            var len = lst.Count();

            // if the quatity requested is greater than the number of items in the list, just return the list
            if (qty > len)
            {
                foreach (var item in lst) yield return item;
            }

            // if the quatity requested is less than 1, return and empty list
            else if (qty < 1)
            {
                yield break;
            }
            else
            {
                var start = len - qty;
                var i = 0;
                foreach (var item in lst)
                {
                    if (i++ < start) continue;
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Converts an IEnumerable<T> to a HashSet<T>, excluding duplicate entries.
        /// </summary>
        /// <typeparam name="T">Type of objects to store in the HashSet</typeparam>
        /// <param name="lst">The list of objects to convert</param>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> lst)
        {
            var hash = new HashSet<T>();
            foreach (var i in lst)
            {
                if (!hash.Contains(i)) hash.Add(i);
            }
            return hash;
        }

        /// <summary>
        /// Merges the second Set into the first and returns the first set with the merged items in it.
        /// </summary>
        /// <typeparam name="T">Type of items stored in the sets</typeparam>
        /// <param name="hsh1">The ISet to merge the items into</param>
        /// <param name="hsh2">The ISet being merged in</param>
        public static ISet<T> Merge<T>(this ISet<T> hsh1, ISet<T> hsh2)
        {
            foreach (var i in hsh2)
            {
                if (!hsh1.Contains(i)) hsh1.Add(i);
            }

            return hsh1;
        }
    }
}