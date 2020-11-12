using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    [Serializable]
    [DataContract]
    public class BranchLookup
    {
        public string BranchCode { get;  set; }
        public string Emp_BranchCode { get;  set; }

        public int BranchId { get;  set; }
        public bool isActive { get;  set; }
        public bool IsValid { get;  set; }
        public BranchLookup()
        {

        }
        public BranchLookup(string branchCode, int branchId, bool isValid)
        {
            this.BranchCode = branchCode;
            this.BranchId = branchId;
            this.IsValid = isValid;
        }

        public BranchLookup(SqlDataReader reader)
        {
            this.BranchCode = reader["branch_code"] as string;
            this.BranchId = reader["branch_id"] as int? ?? 0;

            int branch_status_id = reader["branch_status_id"] as int? ?? 1;
            this.Emp_BranchCode = reader["emp_branch_code"] as string;
            isActive = branch_status_id == 0 ? true : false;
            IsValid = true;
        }
    }
}
