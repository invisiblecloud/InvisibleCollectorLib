using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceCaptureLib.Model
{
    public class Model
    {
        private IDictionary<string, object> _fields;

        protected Model()
        {
            this._fields = new SortedDictionary<string, object>();
        }
    }
}
