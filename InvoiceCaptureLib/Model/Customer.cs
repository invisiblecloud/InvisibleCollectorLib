using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib.Model
{
    public class Customer
    {
        private string address;
        private string city;
        private string country;
        private string email;
        private string externalId;
        private string gid;
        private string name;
        private string phone;
        private string vatNumber;
        private string zipCode;

        public Customer(string myName, string myVatNumber, string myCountry)
        {
            name = myName;
            vatNumber = myVatNumber;
            country = myCountry;
        }

        protected Customer()
        {
        }

        public string Name
        {
            get => name;

            set => name = value;
        }

        public string ExternalId
        {
            get => externalId;

            set => externalId = value;
        }

        public string VatNumber
        {
            get => vatNumber;

            set => vatNumber = value;
        }

        public string Address
        {
            get => address;

            set => address = value;
        }

        public string ZipCode
        {
            get => zipCode;

            set => zipCode = value;
        }

        public string City
        {
            get => city;

            set => city = value;
        }

        public string Country
        {
            get => country;

            set => country = value;
        }

        public string Email
        {
            get => email;

            set => email = value;
        }

        public string Phone
        {
            get => phone;

            set => phone = value;
        }

        public string Gid
        {
            get => gid;

            set => gid = value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }
    }
}