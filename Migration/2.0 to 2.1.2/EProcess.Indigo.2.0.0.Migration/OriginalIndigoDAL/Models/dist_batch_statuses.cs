namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch_statuses
    {
        public dist_batch_statuses()
        {
            dist_batch_status = new HashSet<dist_batch_status>();
            dist_batch_statuses_language = new HashSet<dist_batch_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_batch_statuses_id { get; set; }

        [Required]
        [StringLength(50)]
        public string dist_batch_status_name { get; set; }

        public int? dist_batch_expected_statuses_id { get; set; }

        public virtual ICollection<dist_batch_status> dist_batch_status { get; set; }

        public virtual ICollection<dist_batch_statuses_language> dist_batch_statuses_language { get; set; }
    }
}
