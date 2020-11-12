namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class integration_bellid_batch_sequence
    {
        [Key]
        [Column(Order = 0, TypeName = "date")]
        public DateTime file_generation_date { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short batch_sequence_number { get; set; }
    }
}
