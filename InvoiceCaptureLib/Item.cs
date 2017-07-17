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

        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public double Quantity { get => quantity; set => quantity = value; }
        public double Vat { get => vat; set => vat = value; }
        public double Price { get => price; set => price = value; }
    }
}
