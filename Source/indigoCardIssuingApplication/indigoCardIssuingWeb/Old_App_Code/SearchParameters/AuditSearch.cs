using System;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{    
    [Serializable]
    public class AuditSearch : ISearchParameters
    {
        public string UserName { get; set; }
        public AuditActionType? AuditAction { get; set; }
        public UserRole? Role { get; set; }
        public string keyword { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int? IssuerId { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public bool IsSearch { get; set; }
        public AuditSearch()
        {
        }

        public AuditSearch(string UserName, AuditActionType? auditAction, UserRole? role, string keyword, DateTime dateFrom,
                           DateTime dateTo, int? issuerId, int PageIndex, int rowsPerPage)
        {
            this.UserName = UserName;
            this.AuditAction = auditAction;
            this.keyword = keyword;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.IssuerId = issuerId;
            this.PageIndex = PageIndex;
            this.RowsPerPage = rowsPerPage;
        }
    }
}