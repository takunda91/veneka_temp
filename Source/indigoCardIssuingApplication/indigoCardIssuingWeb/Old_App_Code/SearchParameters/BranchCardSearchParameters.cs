using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class BranchCardSearchParameters : ISearchParameters
    {
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }
        public int? UserRoleId { get; set; }
        public int? ProductId { get; set; }
        public int? PriorityId { get; set; }
        public int? CardIssueMethodId { get; set; }
        public string CardNumber { get; set; }
        public int? BranchCardStatusId { get; set; }
        public long? OperatorUserId { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public BranchCardSearchParameters(int? issuerId, int? branchId, int? userRoleId, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId,
                                                                 long? operatorUserId, int pageIndex, int rowsPerPage)
        {
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.UserRoleId = userRoleId;
            this.ProductId = productId;
            this.PriorityId = priorityId;
            this.CardIssueMethodId = cardIssueMethodId;
            this.CardNumber = cardNumber;
            this.BranchCardStatusId = branchCardStatusId;
            this.OperatorUserId = operatorUserId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
        }
    }
}