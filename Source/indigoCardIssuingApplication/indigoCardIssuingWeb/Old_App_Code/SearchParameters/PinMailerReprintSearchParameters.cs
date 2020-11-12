using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class PinMailerReprintSearchParameters : ISearchParameters
    {
        public int? IssuerId { get; set; }
        public int? BranchId { get; set; }
        public int? UserRoleId { get; set; }
        public int? PinMailerReprintStatusId { get; set; }
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public PinMailerReprintSearchParameters(int? issuerId, int? branchId, int? userRoleId, int? pinMailerReprintStatusId, int pageIndex, int rowsPerPage)
        {
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.UserRoleId = userRoleId;
            this.PinMailerReprintStatusId = pinMailerReprintStatusId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
        }
    }
}