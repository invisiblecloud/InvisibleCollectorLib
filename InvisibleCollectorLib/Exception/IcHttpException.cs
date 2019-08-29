namespace InvisibleCollectorLib.Exception
{
    /// <summary>
    /// Derived from the error JSON returned by collector. Represents the HTTP error code (family 4 or 5) and custom error message
    /// </summary>
    public class IcHttpException : IcException
    {
        public int HttpCode { get; }
        public string ErrorMsg { get; } // should be removed?

        public IcHttpException(int httpCode, string message) : base($"{message} (HTTP Code: {httpCode})")
        {
            HttpCode = httpCode;
            ErrorMsg = message;
        }
    }
}