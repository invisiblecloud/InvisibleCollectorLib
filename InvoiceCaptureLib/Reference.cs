using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceCaptureLib
{
    class Reference
    {

        private String number;
        private String description;
        private Double grossTotal;
        private Double netTotal;
        private Double vat;

        public Reference() { }

        public Reference(String myNumber, Double myGrossTotal, Double myNetTotal, Double myVat )
        {
            this.number = myNumber;
            this.grossTotal = myGrossTotal;
            this.NetTotal = myNetTotal;
            this.vat = myVat;
        }
        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public double GrossTotal
        {
            get
            {
                return grossTotal;
            }

            set
            {
                grossTotal = value;
            }
        }

        public double NetTotal
        {
            get
            {
                return netTotal;
            }

            set
            {
                netTotal = value;
            }
        }

        public double Vat
        {
            get
            {
                return vat;
            }

            set
            {
                vat = value;
            }
        }
    }
}
