using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoFileLoader.objects
{
    class ValidCard
    {
        public string PAN { get; set; }        
        public string BranchCode { get; set; }
        public int BranchId { get; set; }
        public string SequenceNumber { get; set; }
        public string PsuedoPAN { get; set; }

    }
}
