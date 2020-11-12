namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class card_priority
    {
        public card_priority()
        {
            cards = new HashSet<cards>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int card_priority_id { get; set; }

        public int card_priority_order { get; set; }

        [Required]
        [StringLength(50)]
        public string card_priority_name { get; set; }

        public bool default_selection { get; set; }

        public virtual ICollection<cards> cards { get; set; }
    }
}
