using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{
    [Serializable]
    public class PrintBatchSearchParameters : ISearchParameters
    {

        public int? IssuerId { get; set; }
        public int? ProductId { get; set; }

        public string BatchReference { get; set; }
        public int? PrintBatchStatusId { get; set; }
        public int? CardIssueMethodId { get; set; }
        public int? PrintBatchTypeId { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public PrintBatchSearchParameters()
        {
        }

        public PrintBatchSearchParameters(string batchReference, int? printBatchStatusId, int? branchId, int? issuerId, int? productId,
                                         int? cardIssueMethodId, DateTime? dateFrom, DateTime? dateTo, int pageIndex)
        {
            this.BatchReference = batchReference;
            this.PrintBatchStatusId = printBatchStatusId;
            this.BranchId = branchId;
            this.ProductId = productId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.IssuerId = issuerId;
            this.CardIssueMethodId = cardIssueMethodId;
        }
    }
}
