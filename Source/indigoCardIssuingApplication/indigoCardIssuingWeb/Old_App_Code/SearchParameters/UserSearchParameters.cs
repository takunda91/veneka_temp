using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class UserSearchParameters : ISearchParameters
    {
        public int? BranchId { get; set; }
        public BranchStatus? BranchStatus { get; set; }        
        public int? UserStatus { get; set; }

        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
       public string UserName
        {
            get ;set;
        }

        public string BranchName
        {   get; set;
        }

        public string FirstName
        {
             get; set;
        }

        public string LastName
        {
            get; set;
        }

        public int? IssuerID
        {
             get; set;
        }

        public UserRole? UserRole
        {
              get; set;
        }

        public UserSearchParameters(int? issuerId, int? branchId, BranchStatus? branchStatus, int? userStatus, 
                                    UserRole? userRole, string username, string firstName, string lastName, 
                                    int pageIndex, int rowsPerpage)
        {
            this.IssuerID = issuerId;
            this.BranchId = branchId;
            this.BranchStatus = branchStatus;
            this.UserStatus = userStatus;
            this.UserRole = userRole;
            this.UserName = username;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public UserSearchParameters(string userName, int? branchid, string firstName, string lastName, int issuerID,
                          UserRole UserRole)
        {
            this.UserName = userName;
            this.BranchId = branchid;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IssuerID = issuerID;
            this.UserRole = UserRole;
        }

        public UserSearchParameters()
        {
        }

        public UserSearchParameters(int? branchId, BranchStatus? branchStatus, int? userStatus, int pageIndex)
        {
            this.BranchId = branchId;
            this.BranchStatus = branchStatus;
            this.UserStatus = userStatus;
            this.PageIndex = pageIndex;
        }        
    }
}