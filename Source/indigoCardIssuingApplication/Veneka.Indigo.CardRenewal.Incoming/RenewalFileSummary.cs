using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Incoming
{
    public class RenewalFileSummary
    {
        public string FileName { get; set; }

        public int BranchCount { get; set; }

        public int CardCount { get; set; }

        public long RenewalId { get; set; }
    }
}
