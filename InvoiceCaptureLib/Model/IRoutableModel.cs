using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceCaptureLib.Model
{
    interface IRoutableModel
    {
        string RoutableId { get; }
    }
}
