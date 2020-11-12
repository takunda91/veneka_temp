using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.FundsLoad
{
    public class FundsLoadListModel
    {
        public long Id { get; set; }

        public string IssuerCode { get; set; }

        public string IssuerName { get; set; }

        public string BranchCode { get; set; }

        public string BranchName { get; set; }

        public string BankAccountNo { get; set; }

        public string PrepaidAccountNo { get; set; }

        public string PrepaidCardNo { get; set; }

        public decimal Amount { get; set; }

        public int CreatorId { get; set; }
        public DateTime Created { get; set; }

        public int? ReviewerId { get; set; }
        public bool? ReviewerAccepted { get; set; }
        public DateTime? Reviewed { get; set; }

        public int? ApproverId { get; set; }
        public bool? ApproverAccepted { get; set; }
        public DateTime? Approved { get; set; }

        public int? LoaderId { get; set; }
        public DateTime? Loaded { get; set; }

        public DateTime? SMSSentDate { get; set; }

        public FundsLoadStatusType Status { get; set; }

        public int IssuerId { get; set; }

        public int BranchId { get; set; }

        public int ProductId { get; set; }

        public string CreatedBy { get; set; }
        public string ReviewedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string LoadedBy { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

    }
}
