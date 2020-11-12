using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class FundsLoadSearchParameters : ISearchParameters
    {
        public long? UserId { get; set; }
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }

        public FundsLoadStatusType Status { get; set; }
        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public FundsLoadSearchParameters()
        {
        }

        public FundsLoadSearchParameters(long? userId, FundsLoadStatusType statusType, int pageIndex)
        {
            this.UserId = userId;
            this.Status = statusType;
            this.PageIndex = pageIndex;
        }

        public int RowsPerPage {get; set; }
    }
}
