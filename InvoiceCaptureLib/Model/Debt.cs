using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    /// <summary>
    /// The debt model
    /// </summary>
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

        /// <summary>
        /// The currency. Must be an ISO 4217 currency code.
        /// </summary>
        public string Currency
        {
            get => GetField<string>(CurrencyName);

            set => this[CurrencyName] = value;
        }

        /// <summary>
        /// The Id of the customer to whom the debt is issued.
        /// </summary>
        /// <remarks>It must be the customer's <see cref="Customer.Gid"/> itself and not the <see cref="Customer.ExternalId"/></remarks>
        /// <seealso cref="SetCustomerId"/>
        public string CustomerId
        {
            get => GetField<string>(CustomerIdName);

            set => this[CustomerIdName] = value;
        }

        /// <summary>
        /// The debt date. Only the years, month and days are considered.
        /// </summary>
        public DateTime? Date
        {
            get => GetField<DateTime?>(DateName);

            set => this[DateName] = value;
        }

        /// <summary>
        /// The debt due date. Only the years, month and days are considered.
        /// </summary>
        public DateTime? DueDate
        {
            get => GetField<DateTime?>(DueDateName);

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

        /// <summary>
        /// The debt status. Can be one of: "PENDING" - the default value; "PAID"; "CANCELLED"
        /// </summary>
        public string Status
        {
            get => GetField<string>(StatusName);

            set => this[StatusName] = value;
        }

        /// <summary>
        /// The total amount being paid in tax.
        /// </summary>
        public double? Tax
        {
            get => GetField<double?>(TaxName);

            set => this[TaxName] = value;
        }

        /// <summary>
        /// The debt type. Can be one of: "FT", "FS", "SD"
        /// </summary>
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

            var itemRef = KeyRefEquality(ItemsName);
            var attributesRef = KeyRefEquality(AttributesName);

            if (itemRef == false || attributesRef == false)
                return false;
            else if (itemRef == true && attributesRef == true)
                return true;
            else if (itemRef == null && attributesRef == null)
                return left.InternalItems.EqualsCollection(right.InternalItems) &&
                       left.InternalAttributes.EqualsCollection(right.InternalAttributes);
            else if (itemRef == null)
                return left.InternalItems.EqualsCollection(right.InternalItems);
            else
                return left.InternalAttributes.EqualsCollection(right.InternalAttributes);

            bool? KeyRefEquality(string key)
            {
                var leftHas = left._fields.ContainsKey(key);
                var rightHas = right._fields.ContainsKey(key);
                if (leftHas == rightHas && rightHas) // both true, both have key => inconclusive equality
                    return null;
                else if (leftHas == rightHas) // both false, neither have key => equals
                    return true;
                else 
                    return false; // one has and the other doesn't have keu => unequal
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

        /// <summary>
        /// A convenience method to set the customer id.
        /// </summary>
        /// <param name="customer">The customer</param>
        public void SetCustomerId(Customer customer)
        {
            CustomerId = customer.Gid;
        }

        public void UnsetAttributes()
        {
            UnsetField(AttributesName);
        }

        public void UnsetCurrency()
        {
            UnsetField(CurrencyName);
        }

        public void UnsetCustomerId()
        {
            UnsetField(CustomerIdName);
        }

        public void UnsetDate()
        {
            UnsetField(DateName);
        }

        public void UnsetDueDate()
        {
            UnsetField(DueDateName);
        }

        public void UnsetGrossTotal()
        {
            UnsetField(GrossTotalName);
        }

        public void UnsetId()
        {
            UnsetField(IdName);
        }

        public void UnsetItems()
        {
            UnsetField(ItemsName);
        }

        public void UnsetNetTotal()
        {
            UnsetField(NetTotalName);
        }

        public void UnsetNumber()
        {
            UnsetField(NumberName);
        }

        public void UnsetStatus()
        {
            UnsetField(StatusName);
        }

        public void UnsetTax()
        {
            UnsetField(TaxName);
        }

        public void UnsetType()
        {
            UnsetField(TypeName);
        }

        internal void AssertItemsHaveMandatoryFields(params string[] mandatoryFields)
        {
            InternalItems?.ToList().ForEach(entry => entry.AssertHasMandatoryFields(mandatoryFields));
        }

        public override string ToString()
        {
            var fields = Fields;
            fields[ItemsName] = InternalItems?.StringifyList();
            fields[AttributesName] = InternalAttributes?.StringifyDictionary();
            return fields.StringifyDictionary();
        }
    }
}