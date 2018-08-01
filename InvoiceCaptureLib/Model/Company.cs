using System.Collections.Generic;

namespace InvoiceCaptureLib.Model
{
    public class Company : Model, IRoutableModel
    {
        private const string AddressName = "address";
        private const string CityName = "city";
        private const string CountryName = "country";
        private const string IdName = "gid";
        private const string NameName = "name";
        private const string NotificationsName = "notificationsEnabled";
        private const string VatNumberName = "vatNumber";
        private const string ZipCodeName = "zipCode";

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

        public bool? NotificationsEnabled
        {
            get => GetField<bool?>(NotificationsName);

            set => this[NotificationsName] = value;
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

        protected override ISet<string> MandatoryFields =>
            new SortedSet<string> {NameName, VatNumberName};

        protected override ISet<string> SendableFields =>
            new SortedSet<string> {NameName, VatNumberName, AddressName, ZipCodeName, CityName};

        public string RoutableId => Id;

        public override bool Equals(object other)
        {
            return other is Company company && this == company;
        }

        public static bool operator ==(Company left, Company right)
        {
            return (Model) left == (Model) right;
        }

        public static bool operator !=(Company left, Company right)
        {
            return !(left == right);
        }
    }
}