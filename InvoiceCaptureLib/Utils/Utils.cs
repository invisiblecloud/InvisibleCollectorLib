using System.Collections.Generic;
using System.Linq;

namespace InvoiceCaptureLib.Utils
{
    internal static class Utils
    {
        internal static string StringifyDictionary<T, V> (IDictionary<T, V> dictionary)
        {
            return $"{{ {string.Join(", ", dictionary.Select(pair => pair.Key.ToString() + "=" + pair.Value.ToString()).ToArray())} }}";
        }

    }
}
