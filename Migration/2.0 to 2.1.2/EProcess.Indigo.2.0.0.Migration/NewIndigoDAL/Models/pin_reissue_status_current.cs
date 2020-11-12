namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_reissue_status_current
    {
        [Key]
        [Column(Order = 0)]
        public long pin_reissue_status_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long pin_reissue_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_reissue_statuses_id { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "datetime2")]
        public DateTime status_date { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long user_id { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(100)]
        public string audit_workstation { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }
    }
}
