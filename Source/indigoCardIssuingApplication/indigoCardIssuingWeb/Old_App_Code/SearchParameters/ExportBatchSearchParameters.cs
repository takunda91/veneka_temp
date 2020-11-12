using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class ExportBatchSearchParameters : ISearchParameters
    {
        public int? IssuerId { get; set; }
        public int? ProductId { get; set; }
        public long? ExportBatchId { get; set; }
        public string BatchReference { get; set; }
        public int? ExportBatchStatusesId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public ExportBatchSearchParameters()
        {
        }

        public ExportBatchSearchParameters(int? issuerId, int? productId, string batchReference, int? exportBatchStatusesId, 
                                            DateTime? dateFrom, DateTime? dateTo, int pageIndex)
        {
            this.IssuerId = issuerId;
            this.ProductId = productId;
            this.BatchReference = batchReference;
            this.ExportBatchStatusesId = exportBatchStatusesId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
        }
    }
}
