using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InvoiceCaptureLib
{
    public class Invoice
    {
        private String number;
        private String type;
        private DateTime date;
        private DateTime dueDate;
        private Customer customer;
        private Double grossTotal;
        private String currency;

        private String externalId;
        private String status;

        private List<Item> items;
        private Double netTotal;
        private Double tax;

        private List<Reference> references;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public Invoice(String myNumber, String myType, DateTime myDate, DateTime myDueDate, Customer myC, Double myNetTotal, Double myTax, Double myGrossTotal)
        {
            this.number = myNumber;
            this.type = myType;
            this.date = myDate;
            this.dueDate = myDueDate;
            this.customer = myC;
            this.netTotal = myNetTotal;
            this.tax = myTax;
            this.grossTotal = myGrossTotal;
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

        public DateTime DueDate
        {
            get
            {
                return dueDate;
            }

            set
            {
                dueDate = value;
            }
        }

        public Customer Customer
        {
            get
            {
                return customer;
            }

            set
            {
                customer = value;
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

        public List<Item> Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }

        internal List<Reference> References
        {
            get
            {
                return references;
            }

            set
            {
                references = value;
            }
        }

        public string Currency { get => currency; set => currency = value; }
    }
}
