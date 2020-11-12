using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Integration.Send.TMB
{
    public class CardRecordModel
    {
        public string PAN { get; set; }
        public int MBR { get; set; }
        public string REASON { get; set; }
        public int cmCHANGEPAN { get; set; }
    }
}
