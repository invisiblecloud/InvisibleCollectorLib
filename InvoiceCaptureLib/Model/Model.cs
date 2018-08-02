using System;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceCaptureLib.Model
{
    public abstract class Model
    {
        private IDictionary<string, object> _fields;

        protected Model()
        {
            _fields = new Dictionary<string, object>();
        }

        protected object this[string key]
        {
            set => _fields[key] = value;

            private get
            {
                object value = null;
                _fields.TryGetValue(key, out value);
                return value;
            }
        }

        protected virtual ISet<string> MandatoryFields { get; }

        protected virtual ISet<string> SendableFields { get; }

        // don't use this
        internal IDictionary<string, object> Fields
        {
            set => _fields = new Dictionary<string, object>(value);
        }

        internal IDictionary<string, object> SendableDictionary => _fields
            .Where(pair => SendableFields.Contains(pair.Key))
            .ToDictionary(dict => dict.Key, dict => dict.Value);

        public void AssertHasMandatoryFields()
        {
            foreach (var mandatoryField in MandatoryFields)
                if (!_fields.ContainsKey(mandatoryField))
                {
                    var msg = $"Model is missing mandatory field: {mandatoryField}";
                    throw new ArgumentException(msg);
                }
        }

        public override bool Equals(object other)
        {
            return other is Model model && this == model;
        }

        public override int GetHashCode()
        {
            var hash = 0;

            foreach (var entry in _fields)
                hash ^= entry.Key.GetHashCode() ^ entry.Value.GetHashCode();

            return hash;
        }

        public static bool operator ==(Model left, Model right)
        {
            // both null.
            if (left is null && right is null)
                return true;
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null || left._fields == null || right._fields == null)
                return false;
            return left._fields.Count == right._fields.Count && !left._fields.Except(right._fields).Any();
        }

        public static bool operator !=(Model left, Model right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Utils.StringifyDictionary(this._fields);
        }

        protected T GetField<T>(string key)
        {
            return (T) _fields[key];
        }
    }
}