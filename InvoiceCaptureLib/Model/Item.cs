using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib.Model
{
    public class Item
    {
        private string description;
        private string name;
        private double price;
        private double quantity;
        private double vat;

        public Item()
        {
        }

        public Item(string myName, double myVat, double myPrice)
        {
            name = myName;
            vat = myVat;
            price = myPrice;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public double Quantity
        {
            get => quantity;
            set => quantity = value;
        }

        public double Vat
        {
            get => vat;
            set => vat = value;
        }

        public double Price
        {
            get => price;
            set => price = value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }
    }
}