namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class card_issue_method
    {
        public card_issue_method()
        {
            card_issue_method_language = new HashSet<card_issue_method_language>();
            cards = new HashSet<cards>();
            dist_batch = new HashSet<dist_batch>();
            pin_batch = new HashSet<pin_batch>();
            pin_batch_statuses_flow = new HashSet<pin_batch_statuses_flow>();
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int card_issue_method_id { get; set; }

        [Required]
        [StringLength(50)]
        public string card_issue_method_name { get; set; }

        public virtual ICollection<card_issue_method_language> card_issue_method_language { get; set; }

        public virtual ICollection<cards> cards { get; set; }

        public virtual ICollection<dist_batch> dist_batch { get; set; }

        public virtual ICollection<pin_batch> pin_batch { get; set; }

        public virtual ICollection<pin_batch_statuses_flow> pin_batch_statuses_flow { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
