using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class PinReissueSearchParameters : ISearchParameters
    { 
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }        
        public int? PinReissueStatusesId { get; set; }

        public int? PinReissueTypeId { get; set; }
        public long? OperatorUserId { get; set; }
        public bool OperatorInProgress { get; set; }
        public long? AuthoriseUserId { get; set; }
        public byte[] Index { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public PinReissueSearchParameters()
        {
        }

        public PinReissueSearchParameters(int? issuerId, int? branchId, int? pinReissueStatusesId,int? pinReissueTypeId, long? operatorUserId, bool operatorInProgress,
                                            long? authoriseUserId, byte[] index, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage)
        {
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.PinReissueStatusesId = pinReissueStatusesId;
            this.OperatorUserId = operatorUserId;
            this.OperatorInProgress = operatorInProgress;
            this.AuthoriseUserId = authoriseUserId;
            this.Index = index;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
            this.PinReissueTypeId= pinReissueTypeId;
        }
    }
}
