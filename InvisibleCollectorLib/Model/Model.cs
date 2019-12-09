using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public abstract class Model
    {
        protected IDictionary<string, object> _fields;

        protected Model()
        {
            _fields = new Dictionary<string, object>();
        }

        protected Model(Model model)
        {
            _fields = model._fields?.ToDictionary(entry => entry.Key, entry => entry.Value);
        }


        // don't use this, will fail on null value

        /// <summary>
        ///     Test the object for equality with the model.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return other is Model model && this == model;
        }

        /// <summary>
        ///     Compute the model's hashcode
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            var hash = 0;

            foreach (var entry in _fields)
                hash ^= entry.Key.GetHashCode() ^ entry.Value.GetHashCode();

            return hash;
        }

        /// <summary>
        ///     Test models for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Model left, Model right)
        {
            return IcUtils.ReferenceQuality(left, right) ??
                   left._fields.EqualsCollection(right._fields);
        }

        /// <summary>
        ///     Test models for inequality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Model left, Model right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     String representation of the model
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _fields.StringifyDictionary();
        }

        /// <summary>
        ///     Unsets all of the model's fields.
        /// </summary>
        /// <remarks>
        ///     'Unset fields' are fields that are not sent on any requests. They are different from null fields where a null
        ///     value is sent.
        /// </remarks>
        public void UnsetAll()
        {
            _fields = new Dictionary<string, object>();
        }

        public void UnsetField(string fieldName)
        {
            _fields.Remove(fieldName);
        }

        internal IDictionary<string, object> FieldsShallow
        {
            set => _fields = new Dictionary<string, object>(value);

            get => new Dictionary<string, object>(_fields);
        }

        protected object this[string key]
        {
            set => _fields[key] = value;

            private get
            {
                _fields.TryGetValue(key, out var value);
                return value;
            }
        }

        protected T GetField<T>(string key)
        {
            return (T) this[key];
        }

        protected bool? KeyRefEquality(Model other, string key)
        {
            var leftHas = _fields.ContainsKey(key);
            var rightHas = other._fields.ContainsKey(key);
            if (leftHas == rightHas && rightHas) // both true, both have key => inconclusive equality
                return null;
            if (leftHas == rightHas) // both false, neither have key => equals
                return true;
            return false; // one has and the other doesn't have key => unequal
        }
    }
}