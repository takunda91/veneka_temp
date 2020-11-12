using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class PinBatchSearchParameters : ISearchParameters
    { 
        public int? IssuerId { get; set; }
        public string BatchReference { get; set; }
        public int? PinBatchStatusId { get; set; }
        public int? CardIssueMethodId { get; set; }
        public int? PinBatchTypeId { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public PinBatchSearchParameters()
        {
        }

        public PinBatchSearchParameters(string batchReference, int? pinBatchStatusId, int? branchId, int? issuerId,
                                         int? cardIssueMethodId, int? pinBatchTypeId, DateTime? dateFrom, DateTime? dateTo, int pageIndex)
        {
            this.BatchReference = batchReference;
            this.PinBatchStatusId = pinBatchStatusId;
            this.BranchId = branchId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.IssuerId = issuerId;
            this.CardIssueMethodId = cardIssueMethodId;
            this.PinBatchTypeId = pinBatchTypeId;
        }        
    }
}
