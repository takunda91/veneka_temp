namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class terminals
    {
        [Key]
        public int terminal_id { get; set; }

        [Required]
        [StringLength(250)]
        public string terminal_name { get; set; }

        [StringLength(250)]
        public string terminal_model { get; set; }

        [Required]
        public byte[] device_id { get; set; }

        public int branch_id { get; set; }

        public int terminal_masterkey_id { get; set; }

        [StringLength(250)]
        public string workstation { get; set; }

        public DateTime? date_created { get; set; }

        public DateTime? date_changed { get; set; }

        public virtual branch branch { get; set; }

        public virtual masterkeys masterkeys { get; set; }
    }
}
