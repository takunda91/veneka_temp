using System;
using System.Collections.Generic;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalFile
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
        public RenewalStatusType Status { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreateDate { get; set; }

        public List<RenewalDetail> Details { get; set; }
    }
}
