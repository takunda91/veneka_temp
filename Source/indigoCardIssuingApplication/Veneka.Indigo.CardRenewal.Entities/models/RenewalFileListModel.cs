using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalFileListModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
        public RenewalStatusType Status { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreateDate { get; set; }
        public int BranchCount { get; set; }
        public int CardCount { get; set; }
        public int ProductCount { get; set; }
    }
}
