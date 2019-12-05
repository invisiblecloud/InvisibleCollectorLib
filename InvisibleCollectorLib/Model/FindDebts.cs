using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;


namespace InvisibleCollectorLib.Model
{
    /// <summary>
    ///     The model that represent a debt search query
    /// </summary>
    public class FindDebts : Model
    {
        internal const string NumberName = "number";
        internal const string FromDateName = "from_date";
        internal const string ToDateName = "to_date";
        internal const string FromDueDateName = "from_duedate";
        internal const string ToDueDateName = "to_duedate";
        
        
        IDictionary<string,string> Attributes;

        /// <summary>
        ///     This method adds atributes for searching purposes
        /// </summary>
        public void AddAtribute(string key, string value)
        {
            this.Attributes.Add("attributes["+key+"]", value);
        }

        /// <summary>
        ///     This clears all atrributes
        /// </summary>

        public void ClearAttributes()
        {
            this.Attributes.Clear();
        }

        /// <summary>
        ///     Debt number for direct search
        /// </summary>
        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }

        /// <summary>
        ///     The debt date lower limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime? FromDate
        {
            get => GetField<DateTime?>(FromDateName);

            set => this[FromDateName] = value;
        }

        /// <summary>
        ///     The debt date higher limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime? ToDate
        {
            get => GetField<DateTime?>(ToDateName);

            set => this[ToDateName] = value; // datetime is immutable
        }

        /// <summary>
        ///     The due debt date lower limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime? FromDueDate
        {
            get => GetField<DateTime?>(FromDueDateName);

            set => this[FromDueDateName] = value;
        }

        /// <summary>
        ///     The due debt date higher limit. Only the years, month and days are considered.
        /// </summary>
        public DateTime? ToDueDate
        {
            get => GetField<DateTime?>(ToDueDateName);

            set => this[ToDueDateName] = value; // datetime is immutable
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string> {
                NumberName, 
                FromDateName,
                ToDateName,
                FromDueDateName,
                ToDueDateName
            };

        internal IDictionary<string, string> SendableStringDictionary =>
            SendableDictionary.ToDictionary(p =>p.Key, 
            p =>
            {
                if (p.Value is DateTime date)
                    return date.ToString(IcConstants.DateTimeFormat);
                return Convert.ToString(p.Value);
            }).Concat(Attributes).ToDictionary(x => x.Key, x => x.Value);

        public override bool Equals(object other)
        {
            return other is FindDebts find && this == find;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FindDebts left, FindDebts right)
        {
            return left == (Model) right;
        }

        public static bool operator !=(FindDebts left, FindDebts right)
        {
            return !(left == right);
        }
    }
}