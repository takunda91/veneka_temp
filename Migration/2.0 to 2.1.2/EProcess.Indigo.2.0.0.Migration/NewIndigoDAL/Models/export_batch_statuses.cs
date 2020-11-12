namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class export_batch_statuses
    {
        public export_batch_statuses()
        {
            export_batch_status = new HashSet<export_batch_status>();
            export_batch_statuses_language = new HashSet<export_batch_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int export_batch_statuses_id { get; set; }

        [Required]
        [StringLength(100)]
        public string export_batch_statuses_name { get; set; }

        public virtual ICollection<export_batch_status> export_batch_status { get; set; }

        public virtual ICollection<export_batch_statuses_language> export_batch_statuses_language { get; set; }
    }
}
