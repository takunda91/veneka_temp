using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class RemoteCardUpdateSearchParameters : ISearchParameters
    {
        public RemoteCardUpdateSearchParameters(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage)
        {
            this.PAN = pan;
            this.RemoteUpdateStatusesId = remoteUpdateStatusesId;
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.ProductId = productId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
        }
        public string PAN { get; set; }

        public int? RemoteUpdateStatusesId { get; set; }

        public int? IssuerId { get; set; }

        public int? BranchId { get; set; }

        public int? ProductId { get; set; }

        public string RemoteAddress { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }


        public ISearchParameters PreviousSearchParameters { get; set; }

        public int RowsPerPage { get; set; }
    }
}