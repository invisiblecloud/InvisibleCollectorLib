using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib.Model
{
    public class Debt
    {
        private string currency;
        private string customerId;
        private DateTime date;
        private DateTime dueDate;
        private double grossTotal;
        private string id;
        private List<Item> items;
        private double netTotal;
        private string number;
        private string status;
        private double tax;
        private string type;

        public Debt()
        {
        }

        public Debt(string number, string type, string customerId, DateTime date, DateTime dueDate)
        {
            Number = number;
            Type = type;
            CustomerId = customerId;
            Date = date;
            DueDate = dueDate;
        }

        public string Currency
        {
            get => currency;

            set => currency = value;
        }

        public string CustomerId
        {
            get => customerId;

            set => customerId = value;
        }

        public DateTime Date
        {
            get => date;

            set => date = value;
        }

        public DateTime DueDate
        {
            get => dueDate;

            set => dueDate = value;
        }

        public double GrossTotal
        {
            get => grossTotal;

            set => grossTotal = value;
        }

        public string Id
        {
            get => id;

            set => id = value;
        }

        public List<Item> Items
        {
            get => items;

            set => items = value;
        }


        public double NetTotal
        {
            get => netTotal;

            set => netTotal = value;
        }

        public string Number
        {
            get => number;

            set => number = value;
        }

        public string Status
        {
            get => status;

            set => status = value;
        }

        public double Tax
        {
            get => tax;

            set => tax = value;
        }

        public string Type
        {
            get => type;

            set => type = value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }
    }
}