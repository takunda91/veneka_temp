using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{
    [Serializable]
    public class TerminalSearchParams : ISearchParameters
    {
         public int? IssuerId { get; set; }
         public int? BranchId { get; set; }
         public string Terminalname { get; set; }
         public string TerminalModel { get; set; }
         public string DeviceId { get; set; }
         public ISearchParameters PreviousSearchParameters { get; set; }

         public TerminalSearchParams(int? issuerId, int? branchId, string terminalname, string terminalModel, string deviceid)
        {
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.Terminalname = terminalname;
            this.TerminalModel = terminalModel;
            this.DeviceId = deviceid;
            
        }

         public int PageIndex {get; set;}

         public int RowsPerPage {get; set;}

        public bool IsSearch { get; set; }
    }
}