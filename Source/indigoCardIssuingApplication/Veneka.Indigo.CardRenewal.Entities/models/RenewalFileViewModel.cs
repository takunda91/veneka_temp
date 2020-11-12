using System;
using System.Collections.Generic;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalFileViewModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
        public RenewalStatusType Status { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreateDate { get; set; }
        
        public string CreatedByName { get; set; }
        
        public int BranchCount { get; set; }
        public int CardCount { get; set; }
        public int ProductCount { get; set; }

        public List<RenewalDetail> Details { get; set; }
    }
}
