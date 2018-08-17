using System;
using System.Collections.Generic;
using System.Text;

namespace InvisibleCollectorLib.Model
{
    interface IRoutableModel
    {
        /// <summary>
        /// The model's id taht can be used on requests to retrieve models, update models, etc
        /// </summary>
        string RoutableId { get; }
    }
}
