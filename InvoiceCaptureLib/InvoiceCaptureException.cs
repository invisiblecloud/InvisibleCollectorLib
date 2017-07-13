using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceCaptureLib
{
    public class InvoiceCaptureException : Exception
    {
        public InvoiceCaptureException(string message) : base(message)
        {
        }

        public InvoiceCaptureException(SerializationInfo info, StreamingContext context) { }
    }
}
