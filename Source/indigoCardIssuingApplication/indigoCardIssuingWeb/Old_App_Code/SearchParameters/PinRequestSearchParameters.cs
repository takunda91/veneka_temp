using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{
    [Serializable]
    public class PinRequestSearchParameters : ISearchParameters
    {
        public int? IssuerId { get; set; }
        public string PinRequestReference { get; set; }
        public string PinRequestStatus { get; set; }
        public string PinRequestType { get; set; }
        //public int? CardIssueMethodId { get; set; }
        public string PinReissueStatus { get; set; }
        public string PinApprovalStage { get; set; }
        public string PinMaskedPan { get; set; }
        public string ProductBin { get; set; }
        public string LastFourDigits { get; set; }
        public int? ProductId { get; set; }
        public string PinCustomerAccount { get; set; }
        public string PinRequestChannel { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public PinRequestSearchParameters()
        {
        }

        public PinRequestSearchParameters(string pinRequestReference, string pinRequestStatus, int? branchId, int? issuerId, DateTime? dateFrom, DateTime? dateTo, int pageIndex)
        {
            this.PinRequestReference = pinRequestReference;
            this.PinRequestStatus = pinRequestStatus;
            this.BranchId = branchId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.PageIndex = pageIndex;
            this.IssuerId = issuerId;
        }

        public PinRequestSearchParameters(int? issuerId, int pageIndex)
        {
            this.PageIndex = pageIndex;
            this.IssuerId = issuerId;
        }
    }
}