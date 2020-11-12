namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_batch
    {
        public load_batch()
        {
            load_batch_cards = new HashSet<load_batch_cards>();
            load_batch_status = new HashSet<load_batch_status>();
            load_card_failed = new HashSet<load_card_failed>();
        }

        [Key]
        public long load_batch_id { get; set; }

        public long file_id { get; set; }

        public int load_batch_status_id { get; set; }

        public int no_cards { get; set; }

        public DateTime load_date { get; set; }

        [Required]
        [StringLength(100)]
        public string load_batch_reference { get; set; }

        public int load_batch_type_id { get; set; }

        public virtual file_history file_history { get; set; }

        public virtual load_batch_statuses load_batch_statuses { get; set; }

        public virtual ICollection<load_batch_cards> load_batch_cards { get; set; }

        public virtual load_batch load_batch1 { get; set; }

        public virtual load_batch load_batch2 { get; set; }

        public virtual ICollection<load_batch_status> load_batch_status { get; set; }

        public virtual load_batch_types load_batch_types { get; set; }

        public virtual ICollection<load_card_failed> load_card_failed { get; set; }
    }
}
