using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.TMB.CBS.TagPayResponse
{
   public class tagpay
    {
        public string merchantid { get; set; }
        public string client { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string postname { get; set; }
        public string reservebalance { get; set; }
        public string reservecurrency { get; set; }
        public string secondarybalance { get; set; }
        public string secondarycurrency { get; set; }
        public string result { get; set; }
    }
}
