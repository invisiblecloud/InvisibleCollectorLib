using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public class Payment : ItemsModel<PaymentLine>
    {
        internal const string NumberName = "number";
        internal const string CurrencyName = "currency";
        internal const string GrossTotalName = "grossTotal";
        internal const string TypeName = "type";
        internal const string TaxName = "tax";
        internal const string NetTotalName = "netTotal";
        internal const string DateName = "date";
        internal const string StatusName = "status";
        internal const string LinesName = "lines";
        internal const string ExternalIdName = "externalId";


        /// <summary>
        ///     The currency. Must be an ISO 4217 currency code.
        /// </summary>
        public string Currency
        {
            get => GetField<string>(CurrencyName);

            set => this[CurrencyName] = value;
        }

        /// <summary>
        ///     The payment date. Only the years, month and days are considered.
        /// </summary>
        public DateTime? Date
        {
            get => GetField<DateTime?>(DateName);

            set => this[DateName] = value;
        }

        public double? GrossTotal
        {
            get => GetField<double?>(GrossTotalName);

            set => this[GrossTotalName] = value;
        }

        public double? NetTotal
        {
            get => GetField<double?>(NetTotalName);

            set => this[NetTotalName] = value;
        }

        /// <summary>
        ///     The payment status. Can be one of: "FINAL" - default; "CANCELLED"
        /// </summary>
        public string Status
        {
            get => GetField<string>(StatusName);

            set => this[StatusName] = value;
        }

        /// <summary>
        ///     The payment type.
        /// </summary>
        public string Type
        {
            get => GetField<string>(TypeName);

            set => this[TypeName] = value;
        }

        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }

        /// <summary>
        /// The 'id' of the payment, which will be used to identify this payment on future requests.
        /// </summary>
        public string ExternalId
        {
            get => GetField<string>(ExternalIdName);

            set => this[ExternalIdName] = value;
        }

        /// <summary>
        ///     The total amount being paid in tax.
        /// </summary>
        public double? Tax
        {
            get => GetField<double?>(TaxName);

            set => this[TaxName] = value;
        }

        protected override IList<PaymentLine> InternalItems
        {
            get => GetField<IList<PaymentLine>>(LinesName);

            set => this[LinesName] = value;
        }

        /// <summary>
        /// A list of debts being paid.
        /// </summary>
        public IList<PaymentLine> Lines
        {
            get => InternalItems?.Clone();

            set => InternalItems = value?.Clone();
        }

        public void AddLine(PaymentLine line)
        {
            AddItem(line);
        }

        public void UnsetExternalId()
        {
            UnsetField(ExternalId);
        }

        public void UnsetCurrency()
        {
            UnsetField(CurrencyName);
        }

        public void UnsetDate()
        {
            UnsetField(DateName);
        }

        public void UnsetGrossTotal()
        {
            UnsetField(GrossTotalName);
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

        public void UnsetLines()
        {
            UnsetField(LinesName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is Payment payment && this == payment;
        }

        public static bool operator ==(Payment left, Payment right)
        {
            return AreEqual(left, right, LinesName);
        }

        public static bool operator !=(Payment left, Payment right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            var fields = FieldsShallow;
            fields[LinesName] = InternalItems?.StringifyList();
            return fields.StringifyDictionary();
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string>
            {
                NumberName, CurrencyName, GrossTotalName, TypeName, TaxName, NetTotalName, DateName, StatusName,
                LinesName, ExternalIdName
            };

        internal IDictionary<string, object> SendableDictionary()
        {
            var fields = base.SendableDictionary;
            if (InternalItems != null)
                fields[LinesName] = InternalItems.Select(item => item.FieldsShallow).ToList();

            return fields;
        }
    }
}