using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public class Debt : Model, IRoutableModel
    {
        internal const string AttributesName = "attributes";
        internal const string CurrencyName = "currency";
        internal const string CustomerIdName = "customerId";
        internal const string DateName = "date";
        internal const string DueDateName = "dueDate";
        internal const string GrossTotalName = "grossTotal";
        internal const string IdName = "id";
        internal const string ItemsName = "items";
        internal const string NetTotalName = "netTotal";
        internal const string NumberName = "number";
        internal const string StatusName = "status";
        internal const string TaxName = "tax";
        internal const string TypeName = "type";

        public Debt()
        {
        }

        internal Debt(Debt other) : base(other)
        {
            if (other.InternalItems != null)
                InternalItems = other.Items;

            if (other.InternalAttributes != null)
                InternalAttributes = other.Attributes;
        }

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
            get => GetField<IList<Item>>(ItemsName)?.Select(element => (Item) element.Clone()).ToList(); // deep copy

            set => this[ItemsName] = value?.Select(element => (Item) element.Clone()).ToList();
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

        internal override IDictionary<string, object> SendableDictionary
        {
            get
            {
                var fields = base.SendableDictionary;
                if (InternalItems != null)
                    fields[ItemsName] = InternalItems.Select(item => item.SendableDictionary).ToList();

                return fields;
            }
        }

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

        public string RoutableId => Id;

        public void AddItem(Item item)
        {
            if (item is null)
                throw new ArgumentException("Invalid argument");

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
            var refDebt = IcUtils.ReferenceNullableEquals(left, right);
            if (refDebt != null)
                return (bool) refDebt;

            var leftCopy = new Debt(left) {InternalItems = null, InternalAttributes = null};
            var rightCopy = new Debt(right) {InternalItems = null, InternalAttributes = null};

            if (leftCopy != (Model) rightCopy) // compare non collection attributes
                return false;

            return KeyRefEquality(ItemsName) ??
                   KeyRefEquality(AttributesName) ??
                   left.InternalItems.EqualsList(right.InternalItems) &&
                   left.InternalAttributes.EqualsDict(right.InternalAttributes);

            bool? KeyRefEquality(string key)
            {
                var leftHas = left._fields.ContainsKey(key);
                var rightHas = right._fields.ContainsKey(key);
                if (leftHas == rightHas && rightHas) // both true
                    return null;
                if (leftHas == rightHas) // both false
                    return true;
                return false;
            }
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

        public void SetCustomerId(Customer customer)
        {
            CustomerId = customer.Gid;
        }

        internal void AssertItemsHaveMandatoryFields(params string[] mandatoryFields)
        {
            InternalItems?.ToList().ForEach(entry => entry.AssertHasMandatoryFields(mandatoryFields));
        }
    }
}