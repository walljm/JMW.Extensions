using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JMW.Types
{
    public static class Extensions
    {
        public static T CastDelegate<T>(this Delegate src) where T : class
        {
            return (T)(object)Delegate.CreateDelegate(typeof(T), src.Target, src.Method, true); // throw on fail
        }

        public static IEnumerable<PropertyInfo> GetProperties<T>(this Type type) where T : Attribute
        {
            return type.GetProperties()
                       .Where(p => Attribute.IsDefined(p, typeof(T)));
        }

        public static bool HasInterface<T>(this Type type)
        {
            return type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(T));
        }

        public static T GetValue<T>(this PropertyInfo info, object obj)
        {
            return (T)info.GetValue(obj, null);
        }

        public static object GetValue(this PropertyInfo info, object obj)
        {
            return info.GetValue(obj, null);
        }

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