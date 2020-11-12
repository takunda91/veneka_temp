namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch
    {
        public dist_batch()
        {
            dist_batch_cards = new HashSet<dist_batch_cards>();
            dist_batch_status = new HashSet<dist_batch_status>();
            dist_batch_status1 = new HashSet<dist_batch_status>();
        }

        [Key]
        public long dist_batch_id { get; set; }

        public int branch_id { get; set; }

        public int no_cards { get; set; }

        public DateTime date_created { get; set; }

        [Required]
        [StringLength(25)]
        public string dist_batch_reference { get; set; }

        public virtual branch branch { get; set; }

        public virtual ICollection<dist_batch_cards> dist_batch_cards { get; set; }

        public virtual ICollection<dist_batch_status> dist_batch_status { get; set; }

        public virtual ICollection<dist_batch_status> dist_batch_status1 { get; set; }
    }
}
