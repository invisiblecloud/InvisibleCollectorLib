using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public class Payment : Model
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

        public Payment()
        {
        }

        public Payment(Payment other) : base(other)
        {
            if (other.InternalLines != null)
                InternalLines = other.Lines;
        }

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

        private IList<PaymentLine> InternalLines
        {
            get => GetField<IList<PaymentLine>>(LinesName);

            set => this[LinesName] = value;
        }

        /// <summary>
        /// A list of debts being paid.
        /// </summary>
        public IList<PaymentLine> Lines
        {
            get => InternalLines?.Clone();

            set => InternalLines = value?.Clone();
        }

        public void AddLine(PaymentLine line)
        {
            if (line is null)
                throw new ArgumentException("Invalid argument");

            if (InternalLines is null)
                InternalLines = new List<PaymentLine>();

            InternalLines.Add((PaymentLine) line.Clone());
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
            var refDebt = IcUtils.ReferenceQuality(left, right);
            if (refDebt != null)
                return (bool) refDebt;

            var leftCopy = new Payment(left) {InternalLines = null};
            var rightCopy = new Payment(right) {InternalLines = null};

            if (leftCopy != (Model) rightCopy) // compare non collection attributes
                return false;

            var lineReg = left.KeyRefEquality(right, LinesName);
            if (lineReg != null)
                return (bool) lineReg;

            // compare lines
            return left.InternalLines.EqualsCollection(right.InternalLines);
        }

        public static bool operator !=(Payment left, Payment right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            var fields = FieldsShallow;
            fields[LinesName] = InternalLines?.StringifyList();
            return fields.StringifyDictionary();
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string>
            {
                NumberName, CurrencyName, GrossTotalName, TypeName, TaxName, NetTotalName, DateName, StatusName,
                LinesName, ExternalIdName
            };

        internal override IDictionary<string, object> SendableDictionary
        {
            get
            {
                var fields = base.SendableDictionary;
                if (InternalLines != null)
                    fields[LinesName] = InternalLines.Select(item => item.SendableDictionary).ToList();

                return fields;
            }
        }
    }
}