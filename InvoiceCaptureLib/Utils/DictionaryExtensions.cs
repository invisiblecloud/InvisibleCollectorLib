using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvoiceCaptureLib.Utils
{
    internal static class DictionaryExtensions
    {
        internal static bool EqualsDict<K, V>(this IDictionary<K, V> dict1, IDictionary<K, V> dict2)
        {
            return IcUtils.ReferenceNullableEquals(dict1, dict2) ??
                   dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }

        internal static string StringifyDictionary<T, V>(this IDictionary<T, V> dictionary)
        {
            return
                $"{{ {string.Join(", ", dictionary.Select(pair => pair.Key.ToString() + "=" + pair.Value.ToString()).ToArray())} }}";
        }
    }
}
