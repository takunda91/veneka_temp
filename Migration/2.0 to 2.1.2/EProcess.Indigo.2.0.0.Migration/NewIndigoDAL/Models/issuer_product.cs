namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class issuer_product
    {
        public issuer_product()
        {
            cards = new HashSet<cards>();
            integration_cardnumbers = new HashSet<integration_cardnumbers>();
            pin_reissue = new HashSet<pin_reissue>();
            product_fields = new HashSet<product_fields>();
            product_interface = new HashSet<product_interface>();
            currency = new HashSet<currency>();
            card_issue_reason = new HashSet<card_issue_reason>();
            customer_account_type = new HashSet<customer_account_type>();
        }

        [Key]
        public int product_id { get; set; }

        [Required]
        [StringLength(50)]
        public string product_code { get; set; }

        [Required]
        [StringLength(100)]
        public string product_name { get; set; }

        [Required]
        [StringLength(15)]
        public string product_bin_code { get; set; }

        public int issuer_id { get; set; }

        public decimal? name_on_card_top { get; set; }

        public decimal? name_on_card_left { get; set; }

        public int? Name_on_card_font_size { get; set; }

        public int? font_id { get; set; }

        public bool? DeletedYN { get; set; }

        public int src1_id { get; set; }

        public int src2_id { get; set; }

        public int src3_id { get; set; }

        public byte[] PVKI { get; set; }

        public byte[] PVK { get; set; }

        public byte[] CVKA { get; set; }

        public byte[] CVKB { get; set; }

        public int? expiry_months { get; set; }

        public int? fee_scheme_id { get; set; }

        public bool enable_instant_pin_YN { get; set; }

        public int min_pin_length { get; set; }

        public int max_pin_length { get; set; }

        public bool enable_instant_pin_reissue_YN { get; set; }

        public bool cms_exportable_YN { get; set; }

        public int product_load_type_id { get; set; }

        [StringLength(4)]
        public string sub_product_code { get; set; }

        public int pin_calc_method_id { get; set; }

        public bool auto_approve_batch_YN { get; set; }

        public bool account_validation_YN { get; set; }

        public short pan_length { get; set; }

        public bool pin_mailer_printing_YN { get; set; }

        public bool pin_mailer_reprint_YN { get; set; }

        public int? sub_product_id { get; set; }

        public int? master_product_id { get; set; }

        public int card_issue_method_id { get; set; }

        public byte[] decimalisation_table { get; set; }

        public byte[] pin_validation_data { get; set; }

        [StringLength(100)]
        public string pin_block_format { get; set; }

        public virtual card_issue_method card_issue_method { get; set; }

        public virtual ICollection<cards> cards { get; set; }

        public virtual ICollection<integration_cardnumbers> integration_cardnumbers { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual product_service_requet_code1 product_service_requet_code1 { get; set; }

        public virtual product_service_requet_code2 product_service_requet_code2 { get; set; }

        public virtual product_service_requet_code3 product_service_requet_code3 { get; set; }

        public virtual Issuer_product_font Issuer_product_font { get; set; }

        public virtual pin_calc_methods pin_calc_methods { get; set; }

        public virtual product_load_type product_load_type { get; set; }

        public virtual ICollection<pin_reissue> pin_reissue { get; set; }

        public virtual product_fee_scheme product_fee_scheme { get; set; }

        public virtual ICollection<product_fields> product_fields { get; set; }

        public virtual ICollection<product_interface> product_interface { get; set; }

        public virtual ICollection<currency> currency { get; set; }

        public virtual ICollection<card_issue_reason> card_issue_reason { get; set; }

        public virtual ICollection<customer_account_type> customer_account_type { get; set; }
    }
}
