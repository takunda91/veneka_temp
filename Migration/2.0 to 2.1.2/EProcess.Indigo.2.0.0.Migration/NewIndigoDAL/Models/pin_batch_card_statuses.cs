namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_card_statuses
    {
        public pin_batch_card_statuses()
        {
            pin_batch_cards = new HashSet<pin_batch_cards>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_batch_card_statuses_id { get; set; }

        [Required]
        [StringLength(250)]
        public string pin_batch_card_statuses_name { get; set; }

        public virtual ICollection<pin_batch_cards> pin_batch_cards { get; set; }
    }
}
