namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_account
    {
        [Key]
        public long customer_account_id { get; set; }

        public long user_id { get; set; }

        public long card_id { get; set; }

        public int card_issue_reason_id { get; set; }

        public int account_type_id { get; set; }

        public int currency_id { get; set; }

        public int resident_id { get; set; }

        public int customer_type_id { get; set; }

        [Required]
        public byte[] customer_account_number { get; set; }

        [Required]
        public byte[] customer_first_name { get; set; }

        [Required]
        public byte[] customer_middle_name { get; set; }

        [Required]
        public byte[] customer_last_name { get; set; }

        [Required]
        public byte[] name_on_card { get; set; }

        public DateTime date_issued { get; set; }

        [StringLength(50)]
        public string cms_id { get; set; }

        [StringLength(50)]
        public string contract_number { get; set; }

        public int? customer_title_id { get; set; }

        [StringLength(50)]
        public string internal_account_nr { get; set; }

        public virtual card_issue_reason card_issue_reason { get; set; }

        public virtual cards cards { get; set; }

        public virtual customer_title customer_title { get; set; }

        public virtual customer_account_type customer_account_type { get; set; }

        public virtual user user { get; set; }

        public virtual customer_residency customer_residency { get; set; }

        public virtual customer_type customer_type { get; set; }
    }
}
