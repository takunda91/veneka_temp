//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Veneka.Indigo.Common.Models
{
    using System;
    
    public partial class sp_get_issuer_by_code_Result
    {
        public int issuer_id { get; set; }
        public int issuer_status_id { get; set; }
        public int country_id { get; set; }
        public string issuer_name { get; set; }
        public string issuer_code { get; set; }
        public bool auto_create_dist_batch { get; set; }
        public bool instant_card_issue_YN { get; set; }
        public bool pin_mailer_printing_YN { get; set; }
        public bool delete_pin_file_YN { get; set; }
        public bool delete_card_file_YN { get; set; }
        public bool account_validation_YN { get; set; }
        public bool maker_checker_YN { get; set; }
        public string license_file { get; set; }
        public string license_key { get; set; }
        public string cards_file_location { get; set; }
        public string card_file_type { get; set; }
        public string pin_file_location { get; set; }
        public string pin_encrypted_ZPK { get; set; }
        public string pin_mailer_file_type { get; set; }
        public string pin_printer_name { get; set; }
        public string pin_encrypted_PWK { get; set; }
        public Nullable<int> language_id { get; set; }
        public bool card_ref_preference { get; set; }
        public bool classic_card_issue_YN { get; set; }
        public bool enable_instant_pin_YN { get; set; }
        public bool authorise_pin_issue_YN { get; set; }
        public bool authorise_pin_reissue_YN { get; set; }
    }
}