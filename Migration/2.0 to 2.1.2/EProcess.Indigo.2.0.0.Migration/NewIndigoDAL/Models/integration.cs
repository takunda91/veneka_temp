namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("integration")]
    public partial class integration
    {
        public integration()
        {
            integration_object = new HashSet<integration_object>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_id { get; set; }

        [Required]
        [StringLength(150)]
        public string integration_name { get; set; }

        public virtual ICollection<integration_object> integration_object { get; set; }
    }
}
