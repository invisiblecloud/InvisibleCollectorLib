using System;

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
        ///     The payment status. Can be one of: "FINAL"; "CANCELLED"
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
        
    }
}