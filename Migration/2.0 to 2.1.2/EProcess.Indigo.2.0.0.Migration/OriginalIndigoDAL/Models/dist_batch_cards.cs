namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch_cards
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long dist_batch_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long card_id { get; set; }

        public int dist_card_status_id { get; set; }

        public virtual cards cards { get; set; }

        public virtual dist_batch dist_batch { get; set; }

        public virtual dist_card_statuses dist_card_statuses { get; set; }
    }
}
