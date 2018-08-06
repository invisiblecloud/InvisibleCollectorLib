using System.Collections.Generic;

namespace InvoiceCaptureLib.Model
{
    public class Customer : Model, IRoutableModel
    {
        internal const string AddressName = "address";
        internal const string CityName = "city";
        internal const string CountryName = "country";
        internal const string EmailName = "email";
        internal const string ExternalIdName = "externalId";
        internal const string IdName = "gid";
        internal const string NameName = "name";
        internal const string PhoneName = "phone";
        internal const string VatNumberName = "vatNumber";
        internal const string ZipCodeName = "zipCode";

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

        public string ExternalId
        {
            get => GetField<string>(ExternalIdName);

            set => this[ExternalIdName] = value;
        }

        public string Id
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
                PhoneName
            };

        public string RoutableId
        {
            get
            {
                if (!(Id is null) && Id != "")
                    return Id;
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
            return left == (Model) right;
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

        public void UnsetId()
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