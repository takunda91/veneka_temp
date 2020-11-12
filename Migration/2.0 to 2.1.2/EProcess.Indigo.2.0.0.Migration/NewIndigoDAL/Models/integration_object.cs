namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class integration_object
    {
        public integration_object()
        {
            integration_fields = new HashSet<integration_fields>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_object_id { get; set; }

        [Required]
        [StringLength(150)]
        public string integration_object_name { get; set; }

        public virtual integration integration { get; set; }

        public virtual ICollection<integration_fields> integration_fields { get; set; }
    }
}
