using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class DistBatchSearchParameters : ISearchParameters
    {
        public long? UserId { get; set; }
        public int? IssuerId { get; set; }
        public string BatchReference { get; set; }
        public int? DistBatchStatusId { get; set; }
        public int? FlowDistBatchStatusId { get; set; }
        public int? CardIssueMethodId { get; set; }
        public int? DistBatchTypeId { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IncludeOriginBranch { get; set; }
        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public DistBatchSearchParameters()
        {
        }

        public DistBatchSearchParameters(long? userId, string batchReference, int? distBatchStatusId,int? flowDistBatchStatusId ,int? branchId, int? issuerId,
                                         int? cardIssueMethodId, int? distBatchTypeId, DateTime? dateFrom, DateTime? dateTo, bool? includeOriginBranch, int pageIndex)
        {
            this.UserId = userId;
            this.BatchReference = batchReference;
            this.DistBatchStatusId = distBatchStatusId;
            this.BranchId = branchId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.IssuerId = issuerId;
            this.CardIssueMethodId = cardIssueMethodId;
            this.DistBatchTypeId = distBatchTypeId;
            this.IncludeOriginBranch = includeOriginBranch;
            this.FlowDistBatchStatusId= flowDistBatchStatusId;
        }

        public int RowsPerPage {get; set; }
    }
}
