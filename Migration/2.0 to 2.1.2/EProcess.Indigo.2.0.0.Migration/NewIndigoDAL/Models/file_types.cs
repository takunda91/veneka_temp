namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class file_types
    {
        public file_types()
        {
            file_history = new HashSet<file_history>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int file_type_id { get; set; }

        [Required]
        [StringLength(15)]
        public string file_type { get; set; }

        public virtual ICollection<file_history> file_history { get; set; }
    }
}
