using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvisibleCollectorLib.Model
{
    public class Item: Model, ICloneable
    {
        internal const string NameName = "name";

        internal const string DescriptionName = "description";
        internal const string PriceName = "price";
        internal const string QuantityName = "quantity";
        internal const string VatName = "vat";

        private const double FloatingDelta = 0.0001;

        public Item() {}

        private Item(Item other) : base(other) { }

        public string Name
        {
            get => GetField<string>(NameName);

            set => this[NameName] = value;
        }

        public string Description
        {
            get => GetField<string>(DescriptionName);

            set => this[DescriptionName] = value;
        }

        public double? Quantity
        {
            get => GetField<double?>(QuantityName);

            set => this[QuantityName] = value;
        }

        public double? Vat
        {
            get => GetField<double?>(VatName);

            set => this[VatName] = value;
        }

        public double? Price
        {
            get => GetField<double?>(PriceName);

            set => this[PriceName] = value;
        }

        protected override ISet<string> SendableFields => new SortedSet<string> { NameName };

        public override bool Equals(object other)
        {
            return other is Item item && this == item;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            return new Item(this);
        }

        public static bool operator ==(Item left, Item right)
        {
            return left == (Model)right;
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }

        public void UnsetName()
        {
            UnsetField(NameName);
        }

        public void UnsetDescription()
        {
            UnsetField(DescriptionName);
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