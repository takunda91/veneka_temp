namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class cms_connection_config
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string connection_ip { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string connection_port { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string sign_on { get; set; }
    }
}
