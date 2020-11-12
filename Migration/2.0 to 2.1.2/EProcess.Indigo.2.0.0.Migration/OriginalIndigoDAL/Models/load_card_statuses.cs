namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_card_statuses
    {
        public load_card_statuses()
        {
            load_batch_cards = new HashSet<load_batch_cards>();
            load_card_statuses_language = new HashSet<load_card_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int load_card_status_id { get; set; }

        [Required]
        [StringLength(15)]
        public string load_card_status { get; set; }

        public virtual ICollection<load_batch_cards> load_batch_cards { get; set; }

        public virtual ICollection<load_card_statuses_language> load_card_statuses_language { get; set; }
    }
}
