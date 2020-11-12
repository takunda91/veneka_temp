namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class integration_fields
    {
        public integration_fields()
        {
            integration_responses = new HashSet<integration_responses>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_object_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_field_id { get; set; }

        [Required]
        [StringLength(150)]
        public string integration_field_name { get; set; }

        public bool accept_all_responses { get; set; }

        public byte[] integration_field_default_value { get; set; }

        public virtual integration_object integration_object { get; set; }

        public virtual ICollection<integration_responses> integration_responses { get; set; }
    }
}
