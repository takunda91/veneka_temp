using indigoCardIssuingWeb.SearchParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.App_Code.SearchParameters
{
    [Serializable]
    public class FileLoadSearchParameters : ISearchParameters
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int RowsPerPage { get; set; }
        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public FileLoadSearchParameters() { }

        public FileLoadSearchParameters(DateTime dateFrom, DateTime dateTo, int rowsPerPage, int pageIndex)
        {
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.RowsPerPage = rowsPerPage;
            this.PageIndex = pageIndex;
        }
    }
}