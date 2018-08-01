using System.Runtime.Serialization;

namespace InvoiceCaptureLib.Exception
{
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
