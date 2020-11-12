using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoFileLoader.objects
{
    class BranchLookup
    {
        public string BranchCode { get; private set; }
        public int BranchId { get; private set; }
        public bool IsValid { get; private set; }

        public BranchLookup(string branchCode, int branchId, bool isValid)
        {
            this.BranchCode = branchCode;
            this.BranchId = branchId;
            this.IsValid = isValid;
        }
    }
}
