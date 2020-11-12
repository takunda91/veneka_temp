namespace OriginalIndigoDAL.Models
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
            file_history = new HashSet<file_history>();
            flex_affiliate_codes = new HashSet<flex_affiliate_codes>();
            issuer_interface = new HashSet<issuer_interface>();
            issuer_product = new HashSet<issuer_product>();
            mod_interface_account_params = new HashSet<mod_interface_account_params>();
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

        public bool auto_create_dist_batch { get; set; }

        public bool instant_card_issue_YN { get; set; }

        public bool pin_mailer_printing_YN { get; set; }

        public bool delete_pin_file_YN { get; set; }

        public bool delete_card_file_YN { get; set; }

        public bool account_validation_YN { get; set; }

        public bool maker_checker_YN { get; set; }

        [StringLength(100)]
        public string license_file { get; set; }

        [StringLength(1000)]
        public string license_key { get; set; }

        [StringLength(100)]
        public string cards_file_location { get; set; }

        [StringLength(20)]
        public string card_file_type { get; set; }

        [StringLength(100)]
        public string pin_file_location { get; set; }

        [StringLength(40)]
        public string pin_encrypted_ZPK { get; set; }

        [StringLength(20)]
        public string pin_mailer_file_type { get; set; }

        [StringLength(50)]
        public string pin_printer_name { get; set; }

        [StringLength(40)]
        public string pin_encrypted_PWK { get; set; }

        public int? language_id { get; set; }

        public virtual ICollection<branch> branch { get; set; }

        public virtual ICollection<file_history> file_history { get; set; }

        public virtual ICollection<flex_affiliate_codes> flex_affiliate_codes { get; set; }

        public virtual languages languages { get; set; }

        public virtual ICollection<issuer_interface> issuer_interface { get; set; }

        public virtual issuer_statuses issuer_statuses { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }

        public virtual ICollection<mod_interface_account_params> mod_interface_account_params { get; set; }

        public virtual ICollection<user_group> user_group { get; set; }
    }
}
