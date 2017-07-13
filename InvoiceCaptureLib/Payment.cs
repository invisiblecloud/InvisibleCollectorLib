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

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }

        public List<Line> Lines
        {
            get
            {
                return lines;
            }

            set
            {
                lines = value;
            }
        }

        public double NetTotal
        {
            get
            {
                return netTotal;
            }

            set
            {
                netTotal = value;
            }
        }

        public double Tax
        {
            get
            {
                return tax;
            }

            set
            {
                tax = value;
            }
        }

        public double GrossTotal
        {
            get
            {
                return grossTotal;
            }

            set
            {
                grossTotal = value;
            }
        }
        public string Currency { get => currency; set => currency = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public string Status { get => status; set => status = value; }
    }
}
