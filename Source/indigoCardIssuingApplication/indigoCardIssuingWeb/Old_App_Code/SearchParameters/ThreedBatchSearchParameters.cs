using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{
    [Serializable]
    public class ThreedBatchSearchParameters : ISearchParameters
    {
        public string BatchReference { get; set; }
        public int IssuerId { get; set; }
        public ThreedBatchStatus? BatchStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsFromSearch { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }

        public ThreedBatchSearchParameters()
        {
        }

        public ThreedBatchSearchParameters(string batchReference, int issuerId, ThreedBatchStatus? batchStatus, DateTime? dateFrom, DateTime? dateTo,
                                            bool isFromSearch, int pageIndex)
        {
            this.BatchStatus = batchStatus;
            this.BatchReference = batchReference;
            this.IssuerId = issuerId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.IsFromSearch = isFromSearch;
            this.PageIndex = pageIndex;
        }
    }
}
