using System;
using System.Collections.Generic;

namespace InvisibleCollectorLib.Model
{
    public class PaymentLine: Model, ICloneable
    {
        internal const string NumberName = "number";
        internal const string ReferenceNumberName = "referenceNumber";
        internal const string AmountName = "amount";

        public PaymentLine()
        {
        }

        private PaymentLine(PaymentLine other) : base(other)
        {
        }

        protected override ISet<string> SendableFields =>
            new SortedSet<string>
            {
                NumberName, ReferenceNumber, AmountName
            };
        
        public override bool Equals(object other)
        {
            return other is PaymentLine line && this == line;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public object Clone()
        {
            return new PaymentLine(this);
        }
        
        public string Number
        {
            get => GetField<string>(NumberName);

            set => this[NumberName] = value;
        }
        
        public string ReferenceNumber
        {
            get => GetField<string>(ReferenceNumberName);

            set => this[ReferenceNumberName] = value;
        }
        
        public double? Amount
        {
            get => GetField<double?>(AmountName);

            set => this[AmountName] = value;
        }
        
        public void UnsetNumber()
        {
            UnsetField(NumberName);
        }
        
        public void UnsetReferenceNumber()
        {
            UnsetField(ReferenceNumberName);
        }
        
        public void UnsetAmount()
        {
            UnsetField(AmountName);
        }
        
        public static bool operator ==(PaymentLine left, PaymentLine right)
        {
            return left == (Model) right;
        }

        public static bool operator !=(PaymentLine left, PaymentLine right)
        {
            return !(left == right);
        }
    }
}