namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class file_encryption_type
    {
        public file_encryption_type()
        {
            connection_parameters = new HashSet<connection_parameters>();
        }

        [Key]
        public int file_encryption_type_id { get; set; }

        [Column("file_encryption_type")]
        [Required]
        [StringLength(250)]
        public string file_encryption_type1 { get; set; }

        public virtual ICollection<connection_parameters> connection_parameters { get; set; }
    }
}
