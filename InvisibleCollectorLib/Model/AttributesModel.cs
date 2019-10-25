using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public abstract class AttributesModel<ItemT> : ItemsModel<ItemT>
        where ItemT: Model, ICloneable
    {
        protected virtual IDictionary<string, string> InternalAttributes
        {
            get;
            set;
        }

        public IDictionary<string, string> Attributes
        {
            get => InternalAttributes
                ?.ToDictionary(entry => entry.Key, entry => entry.Value);

            set => InternalAttributes = value?.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        
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
        
        public static bool AreEqual<CollectionT, ItemT>(CollectionT left, CollectionT right, string itemsName, string attributesName)
            where CollectionT: AttributesModel<ItemT>, new()
            where ItemT: Model, ICloneable
        {
            var refDebt = IcUtils.ReferenceQuality(left, right);
            if (refDebt != null)
                return (bool) refDebt;

            var leftCopy = new CollectionT()
            {
                FieldsShallow = left.FieldsShallow,
                InternalAttributes = null
            };
            var rightCopy = new CollectionT()
            {
                FieldsShallow = right.FieldsShallow,
                InternalAttributes = null
            };

            if (! ItemsModel<ItemT>.AreEqual<CollectionT, ItemT>(leftCopy, rightCopy, itemsName))
                return false;

            var attributesRef = left.KeyRefEquality(right, attributesName);

            if (attributesRef != null)
                return (bool) attributesRef;
            
            return left.InternalAttributes.EqualsCollection(right.InternalAttributes);
        }
    }
}