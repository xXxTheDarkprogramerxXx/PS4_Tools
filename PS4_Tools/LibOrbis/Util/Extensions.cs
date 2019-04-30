using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4_Tools.LibOrbis.Util
{
    public static class DictionaryExtensions
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> d, K key, V def = default(V))
        {
            if (d.ContainsKey(key)) return d[key];
            return def;
        }
    }

    public static class ArrayExtensions
    {
        public static T[] Fill<T>(this T[] arr, T val)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = val;
            }
            return arr;
        }
    }

}
