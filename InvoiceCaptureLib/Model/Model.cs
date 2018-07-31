using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        protected virtual IImmutableSet<string> MandatoryFields { get; }

        protected virtual IImmutableSet<string> SendableFields { get; }

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

        public override string ToString()
        {
            return
                $"{{ {string.Join(", ", _fields.Select(pair => pair.Key + "=" + pair.Value.ToString()).ToArray())} }}";
        }

        protected T GetField<T>(string key)
        {
            return (T) _fields[key];
        }

    }
}