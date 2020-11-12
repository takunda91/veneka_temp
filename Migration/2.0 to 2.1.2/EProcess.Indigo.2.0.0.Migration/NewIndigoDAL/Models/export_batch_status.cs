namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class export_batch_status
    {
        [Key]
        public long export_batch_status_id { get; set; }

        public long export_batch_id { get; set; }

        public int export_batch_statuses_id { get; set; }

        public long user_id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime status_date { get; set; }

        [Required]
        [StringLength(100)]
        public string comments { get; set; }

        public virtual export_batch export_batch { get; set; }

        public virtual export_batch_statuses export_batch_statuses { get; set; }

        public virtual user user { get; set; }
    }
}
