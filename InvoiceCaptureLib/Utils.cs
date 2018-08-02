using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvoiceCaptureLib
{
    internal static class Utils
    {
        internal static string StringifyDictionary<T, V> (IDictionary<T, V> dictionary)
        {
            return $"{{ {string.Join(", ", dictionary.Select(pair => pair.Key.ToString() + "=" + pair.Value.ToString()).ToArray())} }}";
        }

    }
}
