namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_card_statuses
    {
        public branch_card_statuses()
        {
            branch_card_status = new HashSet<branch_card_status>();
            branch_card_statuses_language = new HashSet<branch_card_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_card_statuses_id { get; set; }

        [Required]
        [StringLength(50)]
        public string branch_card_statuses_name { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status { get; set; }

        public virtual ICollection<branch_card_statuses_language> branch_card_statuses_language { get; set; }
    }
}
