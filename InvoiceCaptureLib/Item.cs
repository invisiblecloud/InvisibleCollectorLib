using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace InvoiceCaptureLib
{
    public class Item
    {
        private String name;
        private String description;
        private Double quantity;
        private Double vat;
        private Double price;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public Item(){ }

        public Item(String myName, Double myVat, Double myPrice)
        {
            this.name = myName;
            this.vat = myVat;
            this.price = myPrice;
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

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public double Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }

        public double Vat
        {
            get
            {
                return vat;
            }

            set
            {
                vat = value;
            }
        }

        public double Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
            }
        }
    }
}
