using System;
using System.Collections.Generic;
using System.Text;

namespace InvisibleCollectorLib.Exception
{
    /// <summary>
    /// Represent a model conflict. Contains the conflicting id.
    /// </summary>
    public class IcModelConflictException : IcException
    {
        /// <summary>
        /// The conflicting id
        /// </summary>
        public string ConflictingId { get; }

        public IcModelConflictException(string message, string id) : base($"{message} (conflicting id: {id})")
        {
            ConflictingId = id;
        }
    }
}
