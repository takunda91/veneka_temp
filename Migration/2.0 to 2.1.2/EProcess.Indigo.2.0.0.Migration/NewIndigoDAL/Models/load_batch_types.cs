namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_batch_types
    {
        public load_batch_types()
        {
            load_batch = new HashSet<load_batch>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int load_batch_type_id { get; set; }

        [Required]
        [StringLength(250)]
        public string load_batch_type { get; set; }

        public virtual ICollection<load_batch> load_batch { get; set; }
    }
}
