using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalHistory
    {
        public long RenewalHistoryId { get; set; }
        public long RenewalId { get; set; }
        public DateTime HistoryDate { get; set; }
        public int CreatorId { get; set; }
        public RenewalStatusType InitialStatus { get; set; }
        public RenewalStatusType FinalStatus { get; set; }
    }
}
