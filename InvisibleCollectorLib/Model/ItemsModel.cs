using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public abstract class ItemsModel<ItemT>: Model
        where ItemT: Model, ICloneable 
    {
        protected virtual IList<ItemT> InternalItems
        {
            get;
            set;
        }

        protected virtual string ItemName { get; }
        
        protected void AddItem(ItemT item)
        {
            if (item == null)
                throw new ArgumentException("Invalid argument");

            if (InternalItems is null)
                InternalItems = new List<ItemT>();

            InternalItems.Add((ItemT) item.Clone());
        }
        
        internal IDictionary<string, object> SendableDictionary()
        {
            var fields = FieldsShallow;
            if (InternalItems != null)
                fields[ItemName] = InternalItems.Select(item => item.FieldsShallow).ToList();

            return fields;
        }

        public static bool AreEqual<CollectionT>(CollectionT left, CollectionT right, string itemsName)
            where CollectionT: ItemsModel<ItemT>, new()
        {
            var refDebt = IcUtils.ReferenceQuality(left, right);
            if (refDebt != null)
                return (bool) refDebt;

            var leftCopy = new CollectionT()
            {
                FieldsShallow = left.FieldsShallow,
                InternalItems = null,
            };
            var rightCopy = new CollectionT()
            {
                FieldsShallow = right.FieldsShallow,
                InternalItems = null, 
            };

            if (leftCopy != (Model) rightCopy) // compare non collection attributes
                return false;

            var lineRef = left.KeyRefEquality(right, itemsName);
            if (lineRef != null)
                return (bool) lineRef;

            // compare lines
            return left.InternalItems.EqualsCollection(right.InternalItems);
        }
        
    }
}