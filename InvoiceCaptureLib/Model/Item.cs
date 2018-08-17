using System;
using System.Collections.Generic;

namespace InvisibleCollectorLib.Model
{
    /// <summary>
    /// Represents a debt's item model
    /// </summary>
    public class Item : Model, ICloneable
    {
        internal const string NameName = "name";
        internal const string DescriptionName = "description";
        internal const string QuantityName = "quantity";
        internal const string VatName = "vat";
        internal const string PriceName = "price";

        public Item()
        {
        }

        private Item(Item other) : base(other)
        {
        }

        public string Description
        {
            get => GetField<string>(DescriptionName);

            set => this[DescriptionName] = value;
        }

        public string Name
        {
            get => GetField<string>(NameName);

            set => this[NameName] = value;
        }

        public double? Price
        {
            get => GetField<double?>(PriceName);

            set => this[PriceName] = value;
        }

        public double? Quantity
        {
            get => GetField<double?>(QuantityName);

            set => this[QuantityName] = value;
        }

        /// <summary>
        /// The VAT percentage (0.0 to 100.0).
        /// </summary>
        public double? Vat
        {
            get => GetField<double?>(VatName);

            set => this[VatName] = value;
        }

        protected override ISet<string> SendableFields => new SortedSet<string> { NameName, PriceName, QuantityName, VatName, DescriptionName };

        public object Clone()
        {
            return new Item(this);
        }

        public override bool Equals(object other)
        {
            return other is Item item && this == item;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Item left, Item right)
        {
            return left == (Model) right;
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }

        public void UnsetDescription()
        {
            UnsetField(DescriptionName);
        }

        public void UnsetName()
        {
            UnsetField(NameName);
        }

        public void UnsetPrice()
        {
            UnsetField(PriceName);
        }

        public void UnsetQuantity()
        {
            UnsetField(QuantityName);
        }

        public void UnsetVat()
        {
            UnsetField(VatName);
        }
    }
}