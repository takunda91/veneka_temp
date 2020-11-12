using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.SearchParameters
{
    [Serializable]
    public sealed class AuthenticationSearchParameters : ISearchParameters
    {
        public int PageIndex { get; set; }

        public int RowsPerPage { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public bool IsSearch { get; set; }
    }
}