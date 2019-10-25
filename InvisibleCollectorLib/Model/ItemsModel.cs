using System.Collections.Generic;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Model
{
    public class ItemsModel<ItemT>: Model
    {
        protected virtual IList<ItemT> InternalItems
        {
            get;
            set;
        }

        public static bool AreEqual<CollectionT, ItemT>(CollectionT left, CollectionT right, string itemsName)
            where CollectionT: ItemsModel<ItemT>, new()
            where ItemT: Model
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