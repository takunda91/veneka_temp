namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch_type
    {
        public dist_batch_type()
        {
            dist_batch = new HashSet<dist_batch>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_batch_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string dist_batch_type_name { get; set; }

        public virtual ICollection<dist_batch> dist_batch { get; set; }
    }
}
