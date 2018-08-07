using System;
using System.Collections.Generic;
using System.Text;

namespace InvisibleCollectorLib.Exception
{
    public class IcModelConflictException : IcException
    {
        public string ConflictingId { get; }

        public IcModelConflictException(string message, string id) : base($"{message} (conflicting id: {id})")
        {
            ConflictingId = id;
        }
    }
}
