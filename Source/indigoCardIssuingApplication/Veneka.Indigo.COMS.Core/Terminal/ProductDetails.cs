using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core.Terminal
{
    public class ProductDetails
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductBIN { get; set; }
        public int PinCalcMethodId { get; set; }
        public int PinBlockFormatId { get; set; }
        public string PVK { get; set; }
        public int PVKI { get; set; }
        public string CVKA { get; set; }
        public string CVKB { get; set; }
        public string DecimalisationTable { get; set; }
        public int MinPinLength { get; set; }
        public int MaxPinLength { get; set; }
    }
}
