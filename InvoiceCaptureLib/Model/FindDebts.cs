using System;

namespace InvisibleCollectorLib.Model
{
    public class FindDebts : Model
    {
        internal const string NumberName = "number";
        internal const string FromDateName = "from_date";
        internal const string ToDateName = "to_date";
        internal const string FromDueDateName = "from_duedate";
        internal const string ToDueDateName = "to_duedate";

        public FindDebts()
        {    
        }
        
        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }
        
        /// <summary>
            /// The debt date lower limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime FromDate
        {
            get => GetField<DateTime>(FromDateName);

            set => this[FromDateName] = value;
        }

        /// <summary>
        /// The debt date higher limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime ToDate
        {
            get => GetField<DateTime>(ToDateName);

            set => this[ToDateName] = value; // datetime is immutable
        }
        
        /// <summary>
        /// The due debt date lower limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime FromDueDate
        {
            get => GetField<DateTime>(FromDueDateName);

            set => this[FromDueDateName] = value;
        }

        /// <summary>
        /// The due debt date higher limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime ToDueDate
        {
            get => GetField<DateTime>(ToDueDateName);

            set => this[ToDueDateName] = value; // datetime is immutable
        }
    }
}