using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    /// <summary>
    /// Used to represent a debts search. Does exact and-wise matching on set fields.
    /// </summary>
    public class FindCustomers : Model
    {
        internal const string EmailName = "email";
        internal const string ExternalIdName = "externalId";
        internal const string PhoneName = "phone";
        internal const string VatNumberName = "vat";

        public string Email
        {
            get => GetField<string>(EmailName);

            set => this[EmailName] = value;
        }
        
        public string ExternalId
        {
            get => GetField<string>(ExternalIdName);

            set => this[ExternalIdName] = value;
        }
        
        public string Phone
        {
            get => GetField<string>(PhoneName);

            set => this[PhoneName] = value;
        }
        
        public string VatNumber
        {
            get => GetField<string>(VatNumberName);

            set => this[VatNumberName] = value;
        }
        
        internal IDictionary<string, string> SendableStringDictionary =>
            FieldsShallow.ToDictionary(p => p.Key, p => Convert.ToString(p.Value));
    }
}