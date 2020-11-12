using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{  [Serializable]
    public class RequestSearchParamters : ISearchParameters
    {
        public long? UserId { get; set; }

        public long? RequestId { get; set; }
        public int? UserRoleId { get; set; }
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }
        public string RequestReferenceNumber { get; set; }
        public string BatchReference { get; set; }
        public int? HybridRequestStatusId { get; set; }
        public int? PrintbatchStatusId { get; set; }
        public long? printbatchId { get; set; }
        public string AccountNumber { get; set; }
        public string FirstName {get; set;}
        public string LastName{ get; set; }
        public string CmsId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public  int? CardIssueMethodId { get; set; }
        public int? ProductId { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public RequestSearchParamters()
        {
        }

        public RequestSearchParamters(int? issuerid, int? branchId, string requestreferencenumber, 
                                    int? requestStatusId,  int? cardIssueMethodId, int? productId,
                                    int pageIndex, int rowsPerPage)
        {
            
            this.BranchId = branchId;
            this.RequestReferenceNumber=requestreferencenumber;
            this.BatchReference = requestreferencenumber;
            this.HybridRequestStatusId = requestStatusId;
          
            this.CardIssueMethodId = cardIssueMethodId;
            this.ProductId = productId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
            this.IssuerId = issuerid;
        }

        
    }
}