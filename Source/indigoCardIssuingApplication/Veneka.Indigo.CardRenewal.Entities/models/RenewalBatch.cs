using System;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalBatch
    {
        public long RenewalBatchId { get; set; }
        public RenewalBatchStatusType RenewalBatchStatus { get; set; }
        public int CardCount { get; set; }
        public int ProductCount { get; set; }
        public int BranchCount { get; set; }
        public DateTime RenewalDate { get; set; }
        public string RenewalBatchReference { get; set; }

        public string CalculatedBatchNumber { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string ProductBin { get; set; }
    }
}
