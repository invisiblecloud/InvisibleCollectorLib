using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace InvoiceCaptureLib
{
    public class Customer
    {

        String name;
        String externalId;
        String vatNumber;
        String address;
        String zipCode;
        String city;
        String country;
        String email;
        String phone;
        String gid;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string ExternalId
        {
            get
            {
                return externalId;
            }

            set
            {
                externalId = value;
            }
        }

        public string VatNumber
        {
            get
            {
                return vatNumber;
            }

            set
            {
                vatNumber = value;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        public string ZipCode
        {
            get
            {
                return zipCode;
            }

            set
            {
                zipCode = value;
            }
        }

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value;
            }
        }

        public string Gid
        {
            get
            {
                return gid;
            }

            set
            {
                gid = value;
            }
        }

        public Customer(String myName, String myVatNumber, String myCountry)
        {
            name = myName;
            vatNumber = myVatNumber;
            country = myCountry;
        }

        protected Customer(){}
    }
}
