namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_card_statuses
    {
        public dist_card_statuses()
        {
            dist_batch_cards = new HashSet<dist_batch_cards>();
            dist_card_statuses_language = new HashSet<dist_card_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_card_status_id { get; set; }

        [Required]
        [StringLength(30)]
        public string dist_card_status_name { get; set; }

        public virtual ICollection<dist_batch_cards> dist_batch_cards { get; set; }

        public virtual ICollection<dist_card_statuses_language> dist_card_statuses_language { get; set; }
    }
}
