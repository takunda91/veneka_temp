using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.CardManagement.objects
{
    public class BatchCardInfo
    {
        public long CardId { get; set; }
        public string PAN { get; set; }
        public string CardReferenceNumber { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string CustomerId { get; set; }
        public long ROW_NO { get; set; }
        public int TOTAL_PAGES { get; set; }
        public int TOTAL_ROWS { get; set; }
    }

    public struct RejectCardInfo
    {
        public long CardId { get; set; }
        public string Comments { get; set; }
    }
}
