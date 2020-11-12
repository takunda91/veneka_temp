namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_batch_statuses
    {
        public load_batch_statuses()
        {
            load_batch = new HashSet<load_batch>();
            load_batch_status = new HashSet<load_batch_status>();
            load_batch_statuses_language = new HashSet<load_batch_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int load_batch_statuses_id { get; set; }

        [Required]
        [StringLength(50)]
        public string load_batch_status_name { get; set; }

        public virtual ICollection<load_batch> load_batch { get; set; }

        public virtual ICollection<load_batch_status> load_batch_status { get; set; }

        public virtual ICollection<load_batch_statuses_language> load_batch_statuses_language { get; set; }
    }
}
