using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace InvoiceCaptureLib
{
    public class Line
    {
        private String number;
        private String referenceNumber;
        private Double amount;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public Line(String myNumber)
        {
            this.number = myNumber;
        }

        public string Number { get => number; set => number = value; }
        public string ReferenceNumber { get => referenceNumber; set => referenceNumber = value; }
        public Double Amount { get => amount; set => amount = value; }
    }
}
