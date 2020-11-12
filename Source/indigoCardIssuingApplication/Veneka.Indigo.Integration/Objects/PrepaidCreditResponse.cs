using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    public class PrepaidCreditResponse
    {
        public string ApprovalCode { get; set; }

        public decimal AvailBalance { get; set; }

        public decimal LedgerBalance { get; set; }
    }
}
