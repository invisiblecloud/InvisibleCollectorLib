using System.Collections.Generic;
using System.Linq;

namespace InvoiceCaptureLib.Utils
{
    internal static class IcUtils
    {
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
    }
}