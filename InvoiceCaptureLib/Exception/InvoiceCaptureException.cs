using System.Runtime.Serialization;

namespace InvoiceCaptureLib.Exception
{
    public class InvoiceCaptureException : System.Exception
    {
        public InvoiceCaptureException(string message) : base(message)
        {
        }

        public InvoiceCaptureException(SerializationInfo info, StreamingContext context) { }
    }
}
