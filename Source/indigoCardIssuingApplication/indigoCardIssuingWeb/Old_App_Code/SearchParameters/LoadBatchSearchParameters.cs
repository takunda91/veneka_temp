using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class LoadBatchSearchParameters : ISearchParameters
    {
        public string BatchReference { get; set; }
        public int IssuerId { get; set; }
        public LoadBatchStatus? LoadBatchStatus { get; set; }        
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsFromSearch { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }

        public LoadBatchSearchParameters()
        {
        }

        public LoadBatchSearchParameters(string batchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? dateFrom, DateTime? dateTo,
                                            bool isFromSearch, int pageIndex)
        {
            this.LoadBatchStatus = loadBatchStatus;
            this.BatchReference = batchReference;
            this.IssuerId = issuerId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.IsFromSearch = isFromSearch;
            this.PageIndex = pageIndex;
        }
    }
}