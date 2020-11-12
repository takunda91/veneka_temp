namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_status
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string batch_reference { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string batch_status { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime status_date { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(25)]
        public string application_user { get; set; }
    }
}
