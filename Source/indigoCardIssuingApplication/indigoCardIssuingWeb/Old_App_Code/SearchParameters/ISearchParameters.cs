using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace indigoCardIssuingWeb.SearchParameters
{
    public interface ISearchParameters
    {
        int PageIndex { get; set; }
        int RowsPerPage { get; set; } 
        ISearchParameters PreviousSearchParameters{get;set;}
        bool IsSearch { get; set; }
    }
}
