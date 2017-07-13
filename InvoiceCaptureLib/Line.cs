using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace InvoiceCaptureLib
{
    public class Line
    {
        private String number;
        private Double amount;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public Line(String myNumber)
        {
            this.number = myNumber;
        }

        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public Double Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }
    }
}