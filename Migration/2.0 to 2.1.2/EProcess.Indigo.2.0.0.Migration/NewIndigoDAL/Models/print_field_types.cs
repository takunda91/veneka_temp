namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class print_field_types
    {
        public print_field_types()
        {
            product_fields = new HashSet<product_fields>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int print_field_type_id { get; set; }

        [StringLength(50)]
        public string print_field_name { get; set; }

        public virtual ICollection<product_fields> product_fields { get; set; }
    }
}
