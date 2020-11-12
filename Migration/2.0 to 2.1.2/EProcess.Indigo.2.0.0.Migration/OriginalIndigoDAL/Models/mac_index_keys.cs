namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mac_index_keys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int table_id { get; set; }

        [Required]
        public byte[] mac_key { get; set; }
    }
}
