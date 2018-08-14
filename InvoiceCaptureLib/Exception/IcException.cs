using System.Runtime.Serialization;

namespace InvisibleCollectorLib.Exception
{
    /// <summary>
    /// The exception used by this library to wrap around other 3rd party libraries and to throw error messages.
    /// </summary>
    public class IcException : System.Exception
    {
        public IcException(string message) : base(message)
        {
        }

        public IcException(string message, System.Exception exception) : base(message, exception)
        {
        }

        public IcException(SerializationInfo info, StreamingContext context) { }
    }
}
