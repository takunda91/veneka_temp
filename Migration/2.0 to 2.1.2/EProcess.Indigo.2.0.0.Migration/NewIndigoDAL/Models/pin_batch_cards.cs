namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_cards
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long pin_batch_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long card_id { get; set; }

        public int pin_batch_cards_statuses_id { get; set; }

        public virtual cards cards { get; set; }

        public virtual pin_batch pin_batch { get; set; }

        public virtual pin_batch_card_statuses pin_batch_card_statuses { get; set; }
    }
}
