using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InvoiceCaptureLib
{
    public class CreditNote
    {
        private String number;
        private DateTime date;
        private Customer customer;
        private Double grossTotal;
        private String currency;

        private String externalId;
        private String status;

        private Double netTotal;
        private Double tax;

        private List<Reference> references;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public CreditNote(String myNumber, DateTime myDate, Customer myC, Double myNetTotal, Double myTax, Double myGrossTotal)
        {
            this.number = myNumber;
            this.date = myDate;
            this.customer = myC;
            this.netTotal = myNetTotal;
            this.tax = myTax;
            this.grossTotal = myGrossTotal;
        }

        public string Number { get => number; set => number = value; }
        public DateTime Date { get => date; set => date = value; }
        public Customer Customer { get => customer; set => customer = value; }
        public double GrossTotal { get => grossTotal; set => grossTotal = value; }

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

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
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
        internal List<Reference> References { get => references; set => references = value; }

        public string Currency { get => currency; set => currency = value; }
    }
}
