namespace InvoiceCaptureLib.Utils
{
    internal static class IcUtils
    {
        // returns null if inconclusive equality
        internal static bool? ReferenceNullableEquals(object left, object right)
        {
            if (left is null && right is null)
                return true;
            if (ReferenceEquals(left, right)) //identity
                return true;
            if (left is null || right is null)
                return false;
            return null;
        }
    }
}