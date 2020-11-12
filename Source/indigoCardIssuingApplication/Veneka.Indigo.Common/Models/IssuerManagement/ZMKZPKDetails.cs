using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Common.Models.IssuerManagement
{
    public class ZMKZPKDetails
    {
        public string Zone { get; set; }
        public string Final { get; set; }
        public string RandomKey { get; set; }
        public string RandomKeyUnderLMK { get; set; }
        public int? issuer_id { get; set; }
    }
}
