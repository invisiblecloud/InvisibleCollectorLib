using System.Collections.Generic;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    /// <summary>
    ///     The customer model.
    /// </summary>
    public class Customer : AttributesModel<CustomerContact>, IRoutableModel
    {
        internal const string AddressName = "address";
        internal const string CityName = "city";
        internal const string CountryName = "country";
        internal const string EmailName = "email";
        internal const string ExternalIdName = "externalId";
        internal const string IdName = "gid";
        internal const string NameName = "name";
        internal const string PhoneName = "phone";
        internal const string MobileName = "mobile";
        internal const string VatNumberName = "vatNumber";
        internal const string ZipCodeName = "zipCode";
        internal const string ContactsName = "contacts";
        internal const string AttributesName = "attributes";

        protected override IDictionary<string, string> InternalAttributes
        {
            get => GetField<IDictionary<string, string>>(AttributesName);

            set => this[AttributesName] = value;
        }
        
        protected override IList<CustomerContact> InternalItems
        {
            get => GetField<IList<CustomerContact>>(ContactsName);

            set => this[ContactsName] = value;
        }
        
        public string Address
        {
            get => GetField<string>(AddressName);

            set => this[AddressName] = value;
        }

        public string City
        {
            get => GetField<string>(CityName);

            set => this[CityName] = value;
        }

        /// <summary>
        ///     The customer's country. Must be a ISO 3166-1 country code
        /// </summary>
        public string Country
        {
            get => GetField<string>(CountryName);

            set => this[CountryName] = value;
        }

        public string Email
        {
            get => GetField<string>(EmailName);

            set => this[EmailName] = value;
        }

        /// <summary>
        ///     Represent a 'secondary' id for the customer assigned by the library user. Used for convenience.
        /// </summary>
        /// <remarks>
        ///     This externalId can be used to retrieve customers from the API. It can be for example the customer id from
        ///     your DB.
        /// </remarks>
        public string ExternalId
        {
            get => GetField<string>(ExternalIdName);

            set => this[ExternalIdName] = value;
        }

        /// <summary>
        ///     The customer id.
        /// </summary>
        public string Gid
        {
            get => GetField<string>(IdName);

            set => this[IdName] = value;
        }

        public string Name
        {
            get => GetField<string>(NameName);

            set => this[NameName] = value;
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

        /// <summary>
        ///     Must be a valid vat number as specified by the <see cref="Country" />.
        /// </summary>
        public string VatNumber
        {
            get => GetField<string>(VatNumberName);

            set => this[VatNumberName] = value;
        }

        public string ZipCode
        {
            get => GetField<string>(ZipCodeName);

            set => this[ZipCodeName] = value;
        }

        public IList<CustomerContact> Contacts
        {
            get => InternalItems?.Clone(); // deep copy

            set => InternalItems = value?.Clone();
        }

        public void AddContact(CustomerContact contact)
        {
            AddItem(contact);
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string>
            {
                NameName,
                ExternalIdName,
                VatNumberName,
                AddressName,
                ZipCodeName,
                CityName,
                CountryName,
                EmailName,
                PhoneName,
                MobileName,
                AttributesName
            };

        public string RoutableId
        {
            get
            {
                if (!(Gid is null) && Gid != "")
                    return Gid;
                return ExternalId;
            }
        }

        public override bool Equals(object other)
        {
            return other is Customer customer && this == customer;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Customer left, Customer right)
        {
            return AreEqual(left, right, ContactsName, AttributesName);
        }

        public static bool operator !=(Customer left, Customer right)
        {
            return !(left == right);
        }

        public void UnsetAddress()
        {
            UnsetField(AddressName);
        }

        public void UnsetCity()
        {
            UnsetField(CityName);
        }

        public void UnsetCountry()
        {
            UnsetField(CountryName);
        }

        public void UnsetEmail()
        {
            UnsetField(EmailName);
        }

        public void UnsetExternalId()
        {
            UnsetField(ExternalIdName);
        }

        public void UnsetGid()
        {
            UnsetField(IdName);
        }

        public void UnsetName()
        {
            UnsetField(NameName);
        }

        public void UnsetPhone()
        {
            UnsetField(PhoneName);
        }

        public void UnsetVatNumber()
        {
            UnsetField(VatNumberName);
        }

        public void UnsetZipCode()
        {
            UnsetField(ZipCodeName);
        }
    }
}