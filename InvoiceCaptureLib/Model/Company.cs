using System.Collections.Generic;
using System.Collections.Immutable;

namespace InvoiceCaptureLib.Model
{
    public class Company : Model
    {
        private const string ADDRESS_NAME = "address";
        private const string CITY_NAME = "city";
        private const string COUNTRY_NAME = "country";
        private const string ID_NAME = "gid";
        private const string NAME_NAME = "name";
        private const string NOTIFICATIONS_NAME = "notificationsEnabled";
        private const string VAT_NUMBER_NAME = "vatNumber";
        private const string ZIP_CODE_NAME = "zipCode";

        public string Address
        {
            get => GetField<string>(ADDRESS_NAME);

            set => this[ADDRESS_NAME] = value;
        }

        public string City
        {
            get => GetField<string>(CITY_NAME);

            set => this[CITY_NAME] = value;
        }

        public string Country
        {
            get => GetField<string>(COUNTRY_NAME);

            set => this[COUNTRY_NAME] = value;
        }

        public string Id
        {
            get => GetField<string>(ID_NAME);

            set => this[ID_NAME] = value;
        }

        public string Name
        {
            get => GetField<string>(NAME_NAME);

            set => this[NAME_NAME] = value;
        }

        public bool? NotificationsEnabled
        {
            get => GetField<bool?>(NOTIFICATIONS_NAME);

            set => this[NOTIFICATIONS_NAME] = value;
        }

        protected override IImmutableSet<string> SendableFields =>
            new HashSet<string> {NAME_NAME, VAT_NUMBER_NAME, ADDRESS_NAME, ZIP_CODE_NAME, CITY_NAME}
                .ToImmutableHashSet();

        public string VatNumber
        {
            get => GetField<string>(VAT_NUMBER_NAME);

            set => this[VAT_NUMBER_NAME] = value;
        }

        public string ZipCode
        {
            get => GetField<string>(ZIP_CODE_NAME);

            set => this[ZIP_CODE_NAME] = value;
        }
    }
}