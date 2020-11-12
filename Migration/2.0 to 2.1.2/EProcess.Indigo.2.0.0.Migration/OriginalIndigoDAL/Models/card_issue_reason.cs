namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class card_issue_reason
    {
        public card_issue_reason()
        {
            card_issue_reason_language = new HashSet<card_issue_reason_language>();
            customer_account = new HashSet<customer_account>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int card_issue_reason_id { get; set; }

        [Required]
        [StringLength(100)]
        public string card_issuer_reason_name { get; set; }

        public virtual ICollection<card_issue_reason_language> card_issue_reason_language { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }
    }
}
