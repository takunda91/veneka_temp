using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Common
{
    public class TerminalCardData
    {
        public int PrimaryKeyId { get; set; }
        public long? PinIssueCardId { get; set; }
        public int? ReissueBranchId { get; set; }
        public int? ProductId { get; set; }

        public string Track2 { get; set; }

        public bool IsTrack2Encrypted { get; set; }

        public string PAN { get; set; }

        public bool IsPANEncrypted { get; set; }
        
        public int PINBlockFormat { get; set; }

        public string PINBlock { get; set; }
        public string approval_status { get; set; }
        public string approval_comment { get; set; }
		public DateTime approval_date { get; set; }
        public DateTime PINFileUploadDate { get; set; }
        public string DummyPan { get; set; }
        public string CardId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ProductName { get; set; }

        public int? TOTAL_PAGES { get; set; }
        public long? ROW_NO { get; set; }
        public int? TOTAL_ROWS { get; set; }
        public int issuer_id { get; set; }

        public int header_pin_file_batch_id { get; set; }
		public string header_batch_reference { get; set; }
		public int header_number_of_cards_on_request { get; set; }
		public int header_issuer_id {get; set; }
		public DateTime header_upload_date {get; set; }
		public string header_approval_status {get; set; }
		public string header_approval_comment {get; set; }
		public DateTime header_approval_date {get; set; }

    }
}
