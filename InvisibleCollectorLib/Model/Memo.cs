using System;

namespace InvisibleCollectorLib.Model
{
    public class Memo : Model
    {
        internal const string NumberName = "number";
        internal const string DescriptionName = "description";
        internal const string DateName = "date";
        internal const string GrossTotalName = "grossTotal";

        protected Memo()
        {
        }

        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }
        
        public string Description
        {
            get => GetField<string>(DescriptionName);

            set => this[DescriptionName] = value;
        }

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
    }
}