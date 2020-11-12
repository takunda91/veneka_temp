using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Common.Models.IssuerManagement
{
    public class PinRequestViewDetails
    {
        public int issuer_id { get; set; }
        public long pin_request_id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public int product_id { get; set; }
        public string product_bin_code { get; set; }
        public string last_for_digit_of_pan { get; set; }
        public string pin_request_reference { get; set; }
        public string pin_request_status { get; set; }
        public string masked_pan { get; set; }
        public int expiry_period { get; set; }
        public int expiry_year { get; set; }
        public string expiry_month { get; set; }
        public string pin_dist_method { get; set; }
        public string account_number { get; set; }
        public string account_email { get; set; }
        public string account_contact { get; set; }
        public string  account_name { get; set; }
        public DateTime pin_create_date { get; set; }
        public string issuer_name { get; set; }
        public string branch_name { get; set; }
        public DateTime last_send_date { get; set; }
        public int? number_of_times_sent { get; set; }
        public DateTime reject_date { get; set; }
        public string reject_reason { get; set; }


    }
}
