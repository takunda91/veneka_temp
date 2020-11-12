using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class CardSearchParameters : ISearchParameters
    {
        public long? UserId { get; set; }
        public int? UserRoleId { get; set; }
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }
        public string CardNumber { get; set; }
        public string CardLastFourDigits { get; set; }
        public string BatchReference { get; set; }
        public int? LoadCardStatusId { get; set; }
        public int? DistCardStatusId { get; set; }
        public int? BranchCardStatusId { get; set; }
        public long? DistBatchId { get; set; }
        public long? ThreedBatchId { get; set; }
        public string AccountNumber { get; set; }
        public string FirstName {get; set;}
        public string LastName{ get; set; }
        public string CmsId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public  int? CardIssueMethodId { get; set; }
        public int? ProductId { get; set; }
        public string CardRefNumber { get; set; }
        public int? PriorityId { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public long? PinbatchId { get; set; }
        public bool IsSearch { get; set; }
        public bool IsRenewalSearch { get; set; }
        public bool IsActivationSearch { get; set; }

        public ISearchParameters PreviousSearchParameters { get; set; }
        public CardSearchParameters()
        {
        }

        public CardSearchParameters(long? userId, int? userRoleId, int? issuerid, int? branchId, string cardNumber, string cardLastFourDigits, string cardRef, string batchReference,
                                    int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId,
                                    string accountNumber, DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId, int? productId, int? priorityId,
                                    int pageIndex, int rowsPerPage)
        {
            this.UserId = userId;
            this.UserRoleId = userRoleId;
            this.BranchId = branchId;
            this.CardNumber = cardNumber;
            this.CardLastFourDigits = cardLastFourDigits;
            this.BatchReference = batchReference;
            this.LoadCardStatusId = loadCardStatusId;
            this.DistCardStatusId = distCardStatusId;
            this.BranchCardStatusId = branchCardStatusId;
            this.AccountNumber = accountNumber;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.CardIssueMethodId = cardIssueMethodId;
            this.ProductId = productId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
            this.IssuerId = issuerid;
            this.CardRefNumber = cardRef;
            this.PriorityId = priorityId;
        }

        public CardSearchParameters( int? branchId,  int? cardIssueMethodId, int? productId, int? issuerid,int? Priority, string cardNumber, 
                                    string accountNumber,
                                    int pageIndex, int rowsPerPage)
        {
            this.CardIssueMethodId = cardIssueMethodId;
            this.AccountNumber = accountNumber;
            this.CardNumber = cardNumber;
            this.ProductId = productId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
            this.IssuerId = issuerid;
            this.BranchId = branchId;
            this.PriorityId = Priority;
        }
    }
}