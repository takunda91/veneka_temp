using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class UserGroupSearchParameters : ISearchParameters
    {
        public int IssuerID{ get; set; }
        public UserRole? UserRole { get; set; }

        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }

        public UserGroupSearchParameters()
        {

        }

        public UserGroupSearchParameters(int issuerID, UserRole? userRole, int pageIndex)
        {
            this.IssuerID = issuerID;
            this.UserRole = userRole;
            this.PageIndex = pageIndex;
        }        
    }
}