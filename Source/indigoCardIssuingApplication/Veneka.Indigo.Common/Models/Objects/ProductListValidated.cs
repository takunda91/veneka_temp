using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Objects
{
    public class ProductValidated
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int SubProductIdLength { get; set; }
        public bool IssuerInstantPinReissue { get; set; }
        public bool InstantPinReissue { get; set; }
        public bool ePinRequest { get; set; }
        public bool ValidLicence { get; set; }
        public string Messages { get; set; }
        public bool ActivationByCenterOperator { get; set; }
    }
}
