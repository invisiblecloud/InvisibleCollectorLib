using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InvoiceCaptureLib
{
    public class Payment
    {
        private String number;
        private String externalId;
        private String type;
        private DateTime date;
        private List<Line> lines;
        private String status;
        private String currency;

        private Double netTotal;
        private Double tax;
        private Double grossTotal;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public Payment() { }
        public Payment(string myNumber, string myType, DateTime myDate, List<Line> myLines, String myCurrency)
        {
            this.number = myNumber;
            this.type = myType;
            this.date = myDate;
            this.Lines = myLines;
            this.currency = myCurrency;
        }

        public string Number { get => number; set => number = value; }
        public string Type { get => type; set => type = value; }
        public DateTime Date { get => date; set =>date = value; }
        public List<Line> Lines { get => lines; set => lines = value; }
        public double NetTotal { get => netTotal; set => netTotal = value; }
        public double Tax { get => tax; set => tax = value; }
        public double GrossTotal { get => grossTotal; set => grossTotal = value; }
        public string Currency { get => currency; set => currency = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public string Status { get => status; set => status = value; }
    }
}
