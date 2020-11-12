namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch
    {
        public pin_batch()
        {
            pin_batch_cards = new HashSet<pin_batch_cards>();
            pin_batch_status = new HashSet<pin_batch_status>();
        }

        [Key]
        public long pin_batch_id { get; set; }

        public int no_cards { get; set; }

        public DateTime date_created { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_batch_reference { get; set; }

        public int pin_batch_type_id { get; set; }

        public int card_issue_method_id { get; set; }

        public int issuer_id { get; set; }

        public int? branch_id { get; set; }

        public virtual branch branch { get; set; }

        public virtual card_issue_method card_issue_method { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<pin_batch_cards> pin_batch_cards { get; set; }

        public virtual pin_batch_type pin_batch_type { get; set; }

        public virtual ICollection<pin_batch_status> pin_batch_status { get; set; }
    }
}
