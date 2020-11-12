namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class cards
    {
        public cards()
        {
            branch_card_status = new HashSet<branch_card_status>();
            customer_account = new HashSet<customer_account>();
            dist_batch_cards = new HashSet<dist_batch_cards>();
            load_batch_cards = new HashSet<load_batch_cards>();
            pin_batch_cards = new HashSet<pin_batch_cards>();
            pin_mailer_reprint = new HashSet<pin_mailer_reprint>();
        }

        [Key]
        public long card_id { get; set; }

        public int product_id { get; set; }

        public int branch_id { get; set; }

        [Required]
        public byte[] card_number { get; set; }

        public int card_sequence { get; set; }

        [Required]
        [MaxLength(20)]
        public byte[] card_index { get; set; }

        public int card_issue_method_id { get; set; }

        public int card_priority_id { get; set; }

        [StringLength(100)]
        public string card_request_reference { get; set; }

        public byte[] card_production_date { get; set; }

        public byte[] card_expiry_date { get; set; }

        public byte[] card_activation_date { get; set; }

        public byte[] pvv { get; set; }

        public decimal? fee_charged { get; set; }

        public bool? fee_waiver_YN { get; set; }

        public bool? fee_editable_YN { get; set; }

        public bool? fee_overridden_YN { get; set; }

        [StringLength(100)]
        public string fee_reference_number { get; set; }

        [StringLength(100)]
        public string fee_reversal_ref_number { get; set; }

        public int origin_branch_id { get; set; }

        public long? export_batch_id { get; set; }

        public virtual branch branch { get; set; }

        public virtual branch branch1 { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status { get; set; }

        public virtual card_issue_method card_issue_method { get; set; }

        public virtual card_priority card_priority { get; set; }

        public virtual issuer_product issuer_product { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<dist_batch_cards> dist_batch_cards { get; set; }

        public virtual export_batch export_batch { get; set; }

        public virtual ICollection<load_batch_cards> load_batch_cards { get; set; }

        public virtual ICollection<pin_batch_cards> pin_batch_cards { get; set; }

        public virtual ICollection<pin_mailer_reprint> pin_mailer_reprint { get; set; }
    }
}
