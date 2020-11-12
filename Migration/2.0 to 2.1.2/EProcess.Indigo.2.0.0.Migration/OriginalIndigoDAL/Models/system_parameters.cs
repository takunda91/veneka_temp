namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class system_parameters
    {
        [Key]
        [Column(Order = 0)]
        public int config_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string config { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string value { get; set; }
    }
}
