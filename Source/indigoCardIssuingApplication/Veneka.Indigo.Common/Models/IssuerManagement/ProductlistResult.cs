namespace Veneka.Indigo.Common.Models
{
    public partial class ProductlistResult
    {
        public int? TOTAL_PAGES { get; set; }
        public long? ROW_NO { get; set; }
        public int? TOTAL_ROWS { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public int product_id { get; set; }
        public string product_bin_code { get; set; }
        public int issuer_id { get; set; }
        public short pan_length { get; set; }
        public string sub_product_code { get; set; }
        public int pin_calc_method_id { get; set; }
        public bool auto_approve_batch_YN { get; set; }
        public bool account_validation_YN { get; set; }
        public decimal? name_on_card_top { get; set; }
        public decimal? name_on_card_left { get; set; }
        public int? Name_on_card_font_size { get; set; }
        public int? font_id { get; set; }
        public string font_name { get; set; }
        public string resource_path { get; set; }
        public string PVK { get; set; }
        public string PVKI { get; set; }
        public string CVKB { get; set; }
        public string CVKA { get; set; }
        public int src1_id { get; set; }
        public int src2_id { get; set; }
        public int src3_id { get; set; }
        public int? expiry_months { get; set; }
        public int? fee_scheme_id { get; set; }
        public bool enable_instant_pin_YN { get; set; }
        public int min_pin_length { get; set; }
        public int max_pin_length { get; set; }
        public bool enable_instant_pin_reissue_YN { get; set; }
        public bool cms_exportable_YN { get; set; }
        public int product_load_type_id { get; set; }
        public bool pin_mailer_printing_YN { get; set; }
        public bool pin_mailer_reprint_YN { get; set; }
        public int card_issue_method_id { get; set; }
        public string decimalisation_table { get; set; }
        public string pin_validation_data { get; set; }
        public bool? DeletedYN { get; set; }
        public int? pin_block_formatid { get; set; }
        public bool charge_fee_to_issuing_branch_YN { get; set; }
        public int production_dist_batch_status_flow { get; set; }
        public int distribution_dist_batch_status_flow { get; set; }
        public bool print_issue_card_YN { get; set; }
        public bool allow_manual_uploaded_YN { get; set; }
        public bool allow_reupload_YN { get; set; }
        public bool remote_cms_update_YN { get; set; }
        public bool? charge_fee_at_cardrequest { get; set; }
        public bool? e_pin_request_YN { get; set; }
        public bool? pin_account_validation_YN { get; set; }
        public bool? renewal_activate_card { get; set; }
        public bool? renewal_charge_card { get; set; }
        public bool? credit_limit_capture { get; set; }
        public bool? credit_limit_update { get; set; }
        public bool? credit_limit_approve { get; set; }

        public bool? email_required { get; set; }

        public bool? generate_contract_number { get; set; }
        public bool? manual_contract_number { get; set; }

        public bool? parallel_approval { get; set; }

        public bool? activation_by_center_operator { get; set; }

        public string credit_contract_prefix { get; set; }
        public string credit_contract_suffix_format { get; set; }
        public long? credit_contract_last_sequence { get; set; }

        public int? production_dist_batch_status_flow_renewal { get; set; }
        public int? distribution_dist_batch_status_flow_renewal { get; set; }
        public bool? Is_mtwenty_printed { get; set; }




    }
}
