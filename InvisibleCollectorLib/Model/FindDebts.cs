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
        internal const string AttributesName = "attributes";

        protected IDictionary<string, string> InternalAttributes
        {
            get => GetField<IDictionary<string, string>>(AttributesName);

            set => this[AttributesName] = value;
        }

        /// <summary>
        ///     Add attribute for searching purposes. Searching is done using OR on attributes.
        /// </summary>
        public void SetAttribute(string key, string value)
        {
            if (InternalAttributes is null)
                InternalAttributes = new Dictionary<string, string>();

            InternalAttributes[key] = value;
        }

        public string GetAttribute(string key)
        {
            return InternalAttributes?[key];
        }

        public IDictionary<string, string> Attributes
        {
            get => InternalAttributes
                ?.ToDictionary(entry => entry.Key, entry => entry.Value);

            set => InternalAttributes = value?.ToDictionary(entry => entry.Key, entry => entry.Value);
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

        internal IDictionary<string, string> SendableStringDictionary
        {
            get
            {
                var copy = new Dictionary<string, object>(_fields);
                copy.Remove(AttributesName);
                if (InternalAttributes != null && InternalAttributes.Count != 0)
                {
                    InternalAttributes.ToList().ForEach(pair => copy[$"attributes[{pair.Key}]"] = pair.Value);
                }

                return copy.ToDictionary(p => p.Key,
                        p =>
                        {
                            if (p.Value is DateTime date)
                                return date.ToString(IcConstants.DateTimeFormat);
                            return Convert.ToString(p.Value);
                        })
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }


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
            var refFind = IcUtils.ReferenceQuality(left, right);
            if (refFind != null)
                return (bool) refFind;

            var leftCopy = new FindDebts()
            {
                FieldsShallow = left.FieldsShallow,
                InternalAttributes = null
            };
            var rightCopy = new FindDebts()
            {
                FieldsShallow = right.FieldsShallow,
                InternalAttributes = null
            };

            if (leftCopy != (Model) rightCopy)
                return false;

            var attributesRef = left.KeyRefEquality(right, AttributesName);
            if (attributesRef != null)
                return (bool) attributesRef;
            
            return left.InternalAttributes.EqualsCollection(right.InternalAttributes);
        }

        public static bool operator !=(FindDebts left, FindDebts right)
        {
            return !(left == right);
        }
    }
}