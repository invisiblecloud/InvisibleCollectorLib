using System;
using System.Collections.Generic;
using System.Linq;

namespace InvisibleCollectorLib.Utils
{
    internal static class CollectionExtensions
    {
        internal static bool EqualsCollection<T>(this ICollection<T> collection1, ICollection<T> collection2)
        {
            return IcUtils.ReferenceQuality(collection1, collection2) ??
                   collection1.Count == collection2.Count && !collection1.Except(collection2).Any();
        }

        internal static string StringifyDictionary<TValue>(this IDictionary<string, TValue> dictionary)
        {
            return
                "{" + string.Join(", ",
                    dictionary.Select(pair => $"{Convert.ToString(pair.Key)} = {Convert.ToString(pair.Value)}")
                        .ToList()) + "}";
        }

        internal static string StringifyList<TValue>(this IList<TValue> list)
        {
            return $"[ {string.Join(",", list.Select(val => Convert.ToString(val)).ToList())} ]";
        }

        internal static IList<T> Clone<T>(this IList<T> list) where T : ICloneable => list.Select(e => (T) e.Clone()).ToList();
        
        internal static IDictionary<K, V> FieldsDifference<K, V>(this IDictionary<K, V> dictionary, params K[] fields) => dictionary
            .Where(pair => ! fields.Contains(pair.Key))
            .ToDictionary(dict => dict.Key, dict => dict.Value);
        
    }
}