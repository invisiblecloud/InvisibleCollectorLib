using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvisibleCollectorLib.Utils
{
    internal static class CollectionExtensions
    {
        internal static bool EqualsCollection<T>(this ICollection<T> collection1, ICollection<T> collection2)
        {
            return IcUtils.ReferenceNullableEquals(collection1, collection2) ??
                   collection1.Count == collection2.Count && !collection1.Except(collection2).Any();
        }

        internal static string StringifyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return
                $"{{ {String.Join(", ", dictionary.Select(pair => Convert.ToString(pair.Key) + "=" + Convert.ToString(pair.Value)).ToArray())} }}";
        }
    }
}
