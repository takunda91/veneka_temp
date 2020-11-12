namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_batch_status
    {
        [Key]
        public long load_batch_status_id { get; set; }

        public long load_batch_id { get; set; }

        public int load_batch_statuses_id { get; set; }

        public long user_id { get; set; }

        public DateTime status_date { get; set; }

        [StringLength(150)]
        public string status_notes { get; set; }

        public virtual load_batch load_batch { get; set; }

        public virtual load_batch_statuses load_batch_statuses { get; set; }

        public virtual user user { get; set; }
    }
}
