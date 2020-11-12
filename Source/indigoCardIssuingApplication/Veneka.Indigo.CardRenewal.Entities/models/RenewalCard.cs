using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalCard
    {
        public long RenewalCardId { get; set; }
        public int ProductId { get; set; }
        public int BranchId { get; set; }
        public string CardNumber { get; set; }
        public int CardSequence { get; set; }
        public string CardIndex { get; set; }
        public int CardIssueMethodId { get; set; }
        public int CardPriorityId { get; set; }
        public string CardRequestReference { get; set; }
        public string CardProductionDate { get; set; }
        public string CardExpiryDate { get; set; }
        public string CardActivationDate { get; set; }
        public string PVV { get; set; }
        public int OriginBranchId { get; set; }
        public long ExportBatchId { get; set; }
        public int OrderingBranchId { get; set; }
        public int DeliveryBranchId { get; set; }
        public decimal FeeAmount { get; set; }
        public bool FeeCharged { get; set; }
        public decimal VatAmount { get; set; }
        public bool VatCharged { get; set; }
        public long RenewalDetailId { get; set; }
    }
}
