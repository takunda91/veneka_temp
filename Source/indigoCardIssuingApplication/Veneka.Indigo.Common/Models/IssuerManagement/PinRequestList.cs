using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Common.Models.IssuerManagement
{
    public partial class PinRequestList
    {
        public int? TOTAL_PAGES { get; set; }
        public long? ROW_NO { get; set; }
        public int? TOTAL_ROWS { get; set; }
        public int issuer_id { get; set; }
        public int pin_request_id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public int product_id { get; set; }
        public string product_bin_code { get; set; }
        public string last_for_digit_of_pan { get; set; }

        public string pin_request_reference { get; set; }
        public string pin_request_status { get; set; }
        public DateTime pin_create_date { get; set; }
        public string issuer_name { get; set; }
        public string branch_name { get; set; }

    }
}
