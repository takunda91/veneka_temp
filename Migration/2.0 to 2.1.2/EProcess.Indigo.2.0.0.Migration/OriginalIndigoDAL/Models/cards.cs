namespace OriginalIndigoDAL.Models
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

        public virtual branch branch { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status { get; set; }

        public virtual issuer_product issuer_product { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<dist_batch_cards> dist_batch_cards { get; set; }

        public virtual ICollection<load_batch_cards> load_batch_cards { get; set; }
    }
}
