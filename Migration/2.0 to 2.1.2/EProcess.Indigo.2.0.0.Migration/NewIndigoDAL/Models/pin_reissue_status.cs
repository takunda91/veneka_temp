namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_reissue_status
    {
        [Key]
        public long pin_reissue_status_id { get; set; }

        public long pin_reissue_id { get; set; }

        public int pin_reissue_statuses_id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime status_date { get; set; }

        public long user_id { get; set; }

        [Required]
        [StringLength(100)]
        public string audit_workstation { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public virtual pin_reissue pin_reissue { get; set; }

        public virtual pin_reissue_statuses pin_reissue_statuses { get; set; }
    }
}
