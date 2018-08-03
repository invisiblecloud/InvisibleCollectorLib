using System.Collections.Generic;
using System.Linq;

namespace InvoiceCaptureLib.Utils
{
    internal static class IcUtils
    {
        internal static bool EqualsDict<K, V>(IDictionary<K, V> dict1, IDictionary<K, V> dict2)
        {
            return ReferenceNullableEquals(dict1, dict2) ??
                   dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }

        // returns null if inconclusive equality
        internal static bool? ReferenceNullableEquals(object left, object right)
        {
            if (left is null && right is null)
                return true;
            else if (ReferenceEquals(left, right)) //identity
                return true;
            else if (left is null || right is null)
                return false;
            else 
                return null;
        }

        internal static string StringifyDictionary<T, V>(IDictionary<T, V> dictionary)
        {
            return
                $"{{ {string.Join(", ", dictionary.Select(pair => pair.Key.ToString() + "=" + pair.Value.ToString()).ToArray())} }}";
        }
    }
}