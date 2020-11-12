namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class issuer_interface
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int interface_type_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int issuer_id { get; set; }

        public int connection_parameter_id { get; set; }

        public virtual connection_parameters connection_parameters { get; set; }

        public virtual interface_type interface_type { get; set; }

        public virtual issuer issuer { get; set; }
    }
}
