namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_status
    {
        [Key]
        public long pin_batch_status_id { get; set; }

        public long pin_batch_id { get; set; }

        public int pin_batch_statuses_id { get; set; }

        public long user_id { get; set; }

        public DateTime status_date { get; set; }

        [StringLength(250)]
        public string status_notes { get; set; }

        public virtual pin_batch pin_batch { get; set; }

        public virtual pin_batch_statuses pin_batch_statuses { get; set; }

        public virtual user user { get; set; }
    }
}
