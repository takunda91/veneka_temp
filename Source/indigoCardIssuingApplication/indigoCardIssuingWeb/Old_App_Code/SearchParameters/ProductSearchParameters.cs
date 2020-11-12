using System;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class ProductSearchParameters : ISearchParameters
    {
        public int IssuerId { get; set; }

        public bool? DeletedYN { get; set; }

        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }

        public ProductSearchParameters()
        {
        }

        public ProductSearchParameters(int issuerId, bool? deletedYN, int pageIndex)
        {
            this.IssuerId = issuerId;
            this.DeletedYN = deletedYN;
            this.PageIndex = pageIndex;
        }
    }
}