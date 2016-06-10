using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JMW.Extensions.Reflection
{
    public static class Extensions
    {
        public static T CastDelegate<T>(this Delegate src) where T : class
        {
            return (T)(object)Delegate.CreateDelegate(typeof(T), src.Target, src.Method, true); // throw on fail
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> that have and attribute of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The Attribute Type you are filtering on</typeparam>
        /// <param name="type">The type to search for properties </param>
        /// <returns><see cref="IEnumerable{PropertyInfo}"/> that have an Attribute of type <typeparamref name="T"/></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetProperties()
                       .Where(p => Attribute.IsDefined(p, typeof(T)));
        }

        /// <summary>
        /// Indicates if the provided type implements the Interface of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of Interface to check for</typeparam>
        /// <param name="type">The type to check</param>
        /// <returns>Boolean indicated if the type implments the interface.</returns>
        public static bool HasInterface<T>(this Type type) where T : class
        {
            return type.GetInterfaces().Any(t => t == typeof(T));
        }

        public static T GetValue<T>(this PropertyInfo info, object obj)
        {
            return (T)info.GetValue(obj, null);
        }
        
        /// <summary>
        /// Safely adds values to a dictionary of lists.  This function handles creating the list if the key is new.
        /// </summary>
        /// <typeparam name="K">Type of the Key</typeparam>
        /// <typeparam name="V">Type of the List value</typeparam>
        /// <param name="dict">Dictionary to add pairs to</param>
        /// <param name="key">The key</param>
        /// <param name="val">The value to add to the list</param>
        public static void SafeAdd<K,V>(this IDictionary<K,List<V>> dict, K key, V val)
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