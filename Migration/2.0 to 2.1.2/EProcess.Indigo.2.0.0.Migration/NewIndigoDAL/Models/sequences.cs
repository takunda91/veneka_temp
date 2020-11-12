namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sequences
    {
        [Key]
        [StringLength(100)]
        public string sequence_name { get; set; }

        public long last_sequence_number { get; set; }

        public DateTime last_updated { get; set; }
    }
}
