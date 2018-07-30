using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib.Model
{
    public class Company
    {
        private string address;
        private string city;
        private string country;
        private string gid;
        private string name;
        private string vatNumber;
        private string zipCode;

        public string Name
        {
            get => name;

            set => name = value;
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