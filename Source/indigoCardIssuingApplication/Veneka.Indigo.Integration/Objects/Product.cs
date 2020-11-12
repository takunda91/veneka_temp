using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BIN { get; set; }
        public string SubProductCode { get; set; }
        public int CardIssueMethodId { get; set; }
        public int ProductLoadTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public string CVKA { get; set; }
        public string CVKB { get; set; }
        public string PVK { get; set; }
        public int PVKI { get; set; }
        public int ServiceCode1 { get; set; }
        public int ServiceCode2 { get; set; }
        public int ServiceCode3 { get; set; }
        public int IssuerId { get; set; }
        public string DecimalisationTable { get; set; }
        public string PinValidationData { get; set; }

        public int PinBlockFormatId { get; set; }
        public int PinCalcMethodId { get; set; }

        public int MinPinLength { get; set; }
        public int MaxPinLength { get; set; }

        public bool HasCreditLimit { get; set; }
    }
}
