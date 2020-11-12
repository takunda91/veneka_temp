using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.SearchParameters
{
    public class FileSearchParameters
    {
        public int IssuerId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public FileSearchParameters(int issuerId, DateTime dateFrom, DateTime dateTo)
        {
            this.IssuerId = issuerId;
            this.DateFrom = dateFrom;
            this.DateTo = DateTo;
        }
    }
}