using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class CardRenewalSearchParameters : ISearchParameters
    {
        public long? UserId { get; set; }
        
        public RenewalStatusType Status { get; set; }
        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public CardRenewalSearchParameters()
        {
        }

        public CardRenewalSearchParameters(long? userId, RenewalStatusType statusType, int pageIndex)
        {
            this.UserId = userId;
            this.Status = statusType;
            this.PageIndex = pageIndex;
        }

        public int RowsPerPage {get; set; }
    }
}
