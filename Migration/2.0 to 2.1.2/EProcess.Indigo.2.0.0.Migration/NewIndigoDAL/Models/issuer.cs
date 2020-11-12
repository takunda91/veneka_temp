namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("issuer")]
    public partial class issuer
    {
        public issuer()
        {
            branch = new HashSet<branch>();
            dist_batch = new HashSet<dist_batch>();
            export_batch = new HashSet<export_batch>();
            file_history = new HashSet<file_history>();
            issuer_interface = new HashSet<issuer_interface>();
            issuer_product = new HashSet<issuer_product>();
            masterkeys = new HashSet<masterkeys>();
            mod_interface_account_params = new HashSet<mod_interface_account_params>();
            pin_batch = new HashSet<pin_batch>();
            pin_reissue = new HashSet<pin_reissue>();
            product_fee_scheme = new HashSet<product_fee_scheme>();
            rswitch_crf_bank_codes = new HashSet<rswitch_crf_bank_codes>();
            user_group = new HashSet<user_group>();
        }

        [Key]
        public int issuer_id { get; set; }

        public int issuer_status_id { get; set; }

        public int country_id { get; set; }

        [Required]
        [StringLength(50)]
        public string issuer_name { get; set; }

        [Required]
        [StringLength(10)]
        public string issuer_code { get; set; }

        public bool instant_card_issue_YN { get; set; }

        public bool maker_checker_YN { get; set; }

        [StringLength(100)]
        public string license_file { get; set; }

        [StringLength(1000)]
        public string license_key { get; set; }

        public int? language_id { get; set; }

        public bool card_ref_preference { get; set; }

        public bool classic_card_issue_YN { get; set; }

        public bool enable_instant_pin_YN { get; set; }

        public bool authorise_pin_issue_YN { get; set; }

        public bool authorise_pin_reissue_YN { get; set; }

        public bool back_office_pin_auth_YN { get; set; }

        public virtual ICollection<branch> branch { get; set; }

        public virtual ICollection<dist_batch> dist_batch { get; set; }

        public virtual ICollection<export_batch> export_batch { get; set; }

        public virtual ICollection<file_history> file_history { get; set; }

        public virtual languages languages { get; set; }

        public virtual ICollection<issuer_interface> issuer_interface { get; set; }

        public virtual issuer_statuses issuer_statuses { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }

        public virtual ICollection<masterkeys> masterkeys { get; set; }

        public virtual ICollection<mod_interface_account_params> mod_interface_account_params { get; set; }

        public virtual ICollection<pin_batch> pin_batch { get; set; }

        public virtual ICollection<pin_reissue> pin_reissue { get; set; }

        public virtual ICollection<product_fee_scheme> product_fee_scheme { get; set; }

        public virtual ICollection<rswitch_crf_bank_codes> rswitch_crf_bank_codes { get; set; }

        public virtual ICollection<user_group> user_group { get; set; }
    }
}
