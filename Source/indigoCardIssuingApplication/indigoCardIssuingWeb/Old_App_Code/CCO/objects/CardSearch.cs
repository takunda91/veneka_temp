using System;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class CardSearch
    {
        public string CardStatus { get; set; }
        public int? branchid { get; set; }
        public int issuerid { get; set; }
       // public WebResponse WebResponse { get; set; }

        public int? Userid { get; set; }

        public string CardNumber { get; set; }
        public string type { get; set; }
        public string BatchReference { get; set; }

        public DateTime DateTo { get; set; }

        public DateTime DateFrom { get; set; }
    }
}