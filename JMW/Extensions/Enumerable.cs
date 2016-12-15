/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
            var enumerable = lst as T[] ?? lst.ToArray();
            var len = enumerable.Count();

            // if the quatity requested is greater than the number of items in the list, just return the list
            if (qty > len)
            {
                foreach (var item in enumerable) yield return item;
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
                foreach (var item in enumerable)
                {
                    if (i++ < start) continue;
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Converts an IEnumerable{T} to a HashSet{T}, excluding duplicate entries.
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

        /// <summary>
        /// Safely adds values to a dictionary of lists.  This function handles creating the list if the key is new.
        /// </summary>
        /// <typeparam name="K">Type of the Key</typeparam>
        /// <typeparam name="V">Type of the List value</typeparam>
        /// <param name="dict">Dictionary to add pairs to</param>
        /// <param name="key">The key</param>
        /// <param name="val">The value to add to the list</param>
        public static void SafeAdd<K, V>(this IDictionary<K, List<V>> dict, K key, V val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(val);
            }
            else
            {
                dict.Add(key, new List<V>() { val });
            }
        }

        public static bool AddIfNotPresent<K, V>(this IDictionary<K, V> dict, K key, V val)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, val);
                return true;
            }

            return false;
        }
    }
}