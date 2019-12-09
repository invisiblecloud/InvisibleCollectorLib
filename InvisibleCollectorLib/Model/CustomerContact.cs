using System;
using System.Collections.Generic;

namespace InvisibleCollectorLib.Model
{
    public class CustomerContact : Model, ICloneable
    {
        internal const string NameName = "name";
        internal const string EmailName = "email";
        internal const string PhoneName = "phone";
        internal const string MobileName = "mobile";
        internal const string IdName = "id";

        public CustomerContact()
        {
            
        }

        private CustomerContact(CustomerContact other) : base(other)
        {
            
        }
        
        public string Name
        {
            get => GetField<string>(NameName);

            set => this[NameName] = value;
        }

        public string Email
        {
            get => GetField<string>(EmailName);

            set => this[EmailName] = value;
        }

        public string Phone
        {
            get => GetField<string>(PhoneName);

            set => this[PhoneName] = value;
        }

        public string Mobile
        {
            get => GetField<string>(MobileName);

            set => this[MobileName] = value;
        }

        public string Id
        {
            get => GetField<string>(IdName);

            set => this[IdName] = value;
        }

        public object Clone()
        {
            return new CustomerContact(this);
        }
    }
}