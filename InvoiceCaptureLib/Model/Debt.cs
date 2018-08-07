using System;
using System.Collections.Generic;
using System.Linq;

namespace InvisibleCollectorLib.Model
{
    public class Debt : Model
    {
        private const string AttributesName = "attributes";
        private const string CurrencyName = "currency";
        private const string CustomerIdName = "customerId";
        private const string DateName = "date";
        private const string DueDateName = "dueDate";
        private const string GrossTotalName = "grossTotal";
        private const string IdName = "id";
        private const string ItemsName = "items";
        private const string NetTotalName = "netTotal";
        private const string NumberName = "number";
        private const string StatusName = "status";
        private const string TaxName = "tax";
        private const string TypeName = "type";

        public IDictionary<string, string> Attributes
        {
            get => GetField<IDictionary<string, string>>(AttributesName)
                ?.ToDictionary(entry => entry.Key, entry => entry.Value);

            set => this[AttributesName] = value?.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        public string Currency
        {
            get => GetField<string>(CurrencyName);

            set => this[CurrencyName] = value;
        }

        public string CustomerId
        {
            get => GetField<string>(CustomerIdName);

            set => this[CustomerIdName] = value;
        }

        public DateTime Date
        {
            get => GetField<DateTime>(DateName);

            set => this[DateName] = value;
        }

        public DateTime DueDate
        {
            get => GetField<DateTime>(DueDateName);

            set => this[DueDateName] = value; // datetime is immutable
        }

        public double? GrossTotal
        {
            get => GetField<double?>(GrossTotalName);

            set => this[GrossTotalName] = value;
        }

        public string Id
        {
            get => GetField<string>(IdName);

            set => this[IdName] = value;
        }

        public IList<Item> Items
        {
            get
            {
                var items = GetField<IList<Item>>(ItemsName);
                return items is null ? null : new List<Item>(items.Select(v => (Item) v.Clone())); // deep copy
            }

            set => this[ItemsName] =
                value is null ? null : new List<Item>(value.Select(v => (Item) v.Clone())); // deep copy
        }

        public double? NetTotal
        {
            get => GetField<double?>(NetTotalName);

            set => this[NetTotalName] = value;
        }

        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }

        public string Status
        {
            get => GetField<string>(StatusName);

            set => this[StatusName] = value;
        }

        public double? Tax
        {
            get => GetField<double?>(TaxName);

            set => this[TaxName] = value;
        }

        public string Type
        {
            get => GetField<string>(TypeName);

            set => this[TypeName] = value;
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string>
            {
                NumberName,
                CustomerIdName,
                TypeName,
                StatusName,
                DateName,
                DueDateName,
                NetTotalName,
                TaxName,
                GrossTotalName,
                CurrencyName,
                ItemsName,
                AttributesName
            };

        private IDictionary<string, string> InternalAttributes
        {
            get => GetField<IDictionary<string, string>>(AttributesName);

            set => this[AttributesName] = value;
        }

        private IList<Item> InternalItems
        {
            get => GetField<IList<Item>>(ItemsName);

            set => this[ItemsName] = value;
        }

        public void AddItem(Item item)
        {
            if (InternalItems is null)
                InternalItems = new List<Item>();

            InternalItems.Add((Item) item.Clone());
        }

        public override bool Equals(object other)
        {
            return other is Debt debt && this == debt;
        }

        public string GetAttribute(string key)
        {
            return InternalAttributes?[key];
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Debt left, Debt right)
        {
            return left == (Model) right;
        }

        public static bool operator !=(Debt left, Debt right)
        {
            return !(left == right);
        }

        public void SetAttribute(string key, string value)
        {
            if (InternalAttributes is null)
                InternalAttributes = new Dictionary<string, string>();

            InternalAttributes[key] = value;
        }
    }
}